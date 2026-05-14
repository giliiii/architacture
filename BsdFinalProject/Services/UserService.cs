using BsdFinalProject.DTOs;
using BsdFinalProject.IRepositories;
using BsdFinalProject.IServices;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;
using Chocolate.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BsdFinalProject.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _repo;
        private readonly IConfiguration _config;
        private readonly ILogger<UserService> _logger;
        private readonly ICacheService _cache;

        //public UserService(IUserRepository repo, IConfiguration config, ILogger<UserService> logger)
        //{
        //    _config = config;
        //    _logger = logger;
        //    _repo = (UserRepository?)repo;  // ����� �-UserRepository ������
        //}
        public UserService(ILogger<UserService> logger, IConfiguration config, SaleContextFactory saleContextFactory, ICacheService cache)
        {
            _logger = logger;
            _repo = new UserRepository(saleContextFactory);
            _config = config;
            _cache = cache;

        }

        public async Task<(bool Success, string? Token, string? Error)> UserRegister(CreateUserDto dto)
        {
            
            _logger.LogInformation("start Registering user with email: {Email}", dto.EMail);
            if (string.IsNullOrWhiteSpace(dto.EMail) || string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.LogWarning("Registration failed: Email or password is empty.");
                return (false, null, "Email and password are required.");
            }
            _logger.LogInformation("Checking if email {Email} is already in use.", dto.EMail);
            var existing = await _repo.GetByEmail(dto.EMail);
            if (existing != null) {
                _logger.LogWarning("Registration failed: Email {Email} is already in use.", dto.EMail);
                return (false, null, "Email already in use."); }

            // hash password
            _logger.LogInformation("Hashing password for email: {Email}", dto.EMail);
            var hashed = HashPassword(dto.Password);
            _logger.LogInformation("try Creating new user record for email: {Email}", dto.EMail);

            var user = new User
            {
                FullName = dto.FullName,
                EMail = dto.EMail,
                Phone = dto.Phone,
                Address = dto.Address,
                Password = hashed,
            };

            var created = await _repo.CreateUser(user);
            _logger.LogInformation("User registered successfully with email: {Email}", dto.EMail);

            // Invalidate cache for the new user
            var cacheKey = $"user:{dto.EMail}";
            await _cache.RemoveAsync(cacheKey);

            var token = CreateToken(created);
            _logger.LogInformation("JWT token created for user with email: {Email}", dto.EMail);
            return (true, token, null);
        }

        // New: Login implementation
        public async Task<(bool Success, string? Token, string? Error)> LoginAsync(LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.EMail) || string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.LogWarning("Login failed: Email or password is empty.");
                return (false, null, "Email and password are required.");
            }
            _logger.LogInformation("Attempting to log in user with email: {Email}", dto.EMail);

            var cacheKey = $"user:{dto.EMail}";
            var user = await _cache.GetAsync<User>(cacheKey);
            if (user == null)
            {
                user = await _repo.GetByEmail(dto.EMail);
                if (user != null)
                {
                    await _cache.SetAsync(cacheKey, user, TimeSpan.FromMinutes(30));
                }
            }

            if (user == null) { 
                _logger.LogWarning("Login failed: No user found with email: {Email}", dto.EMail);
                return (false, null, "Invalid credentials.");
            }

            if (string.IsNullOrEmpty(user.Password) || !VerifyPassword(user.Password, dto.Password))
            {
                _logger.LogWarning("Login failed: Invalid password for email: {Email}", dto.EMail);
                return (false, null, "Invalid credentials.");
            }
            _logger.LogInformation("Password verified for user with email: {Email}", dto.EMail);
            var token = CreateToken(user);
            _logger.LogInformation("User logged in successfully with email: {Email}", dto.EMail);
            return (true, token, null);
        }

        private string CreateToken(User user)
        {
            _logger.LogInformation("start Creating JWT token for user with email: {Email}", user.EMail);
            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireMinutes = int.TryParse(_config["Jwt:ExpireMinutes"], out var m) ? m : 60;

            if (string.IsNullOrEmpty(key)) {
                _logger.LogError("JWT Key not configured.");
                throw new InvalidOperationException("JWT Key not configured."); }

            var claims = new[]
            {
                //new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                //new Claim(JwtRegisteredClaimNames.Email, user.EMail ?? string.Empty),
                //new Claim("name", user.FullName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.EMail ?? string.Empty),
                new Claim("name", user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString()),

            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // PBKDF2 hashing (salt + hash stored as: iterations.saltBase64.hashBase64)
        private  string HashPassword(string password, int iterations = 100_000)
        {
            _logger.LogInformation("start Hashing password using PBKDF2.");
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public  bool VerifyPassword(string hashedPassword, string password)
        {
            _logger.LogInformation("start Verifying password using PBKDF2.");
            var parts = hashedPassword.Split('.', 3);
            if (parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out var iterations)) return false;
            var salt = Convert.FromBase64String(parts[1]);
            var hash = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(hash.Length);

            return CryptographicOperations.FixedTimeEquals(computed, hash);
        }
    }
}