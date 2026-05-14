//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;
using BsdFinalProject.IServices;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;
using Chocolate.Data;

namespace BsdFinalProject.Services
{
    public class DonorService : IDonorService
    {
        private readonly DonorRepository _repository;
        private readonly ILogger<DonorService> _logger;
        private readonly ICacheService _cache;

        public DonorService(ILogger<DonorService> logger,SaleContextFactory saleContextFactory, ICacheService cache)
        {
            _logger = logger;
            _repository = new DonorRepository(saleContextFactory);
            _cache = cache;
        }

        public async Task<DonorDto?> GetDonorById(int id)
        {
            _logger.LogInformation($"Fetching donor with id {id}");

            var cacheKey = $"donor:{id}";
            var cachedDonor = await _cache.GetAsync<DonorDto>(cacheKey);
            if (cachedDonor != null)
            {
                _logger.LogInformation($"Donor retrieved from cache with id {id}");
                return cachedDonor;
            }

            var d = await _repository.GetDonorById(id);
            if (d == null)
            {
                _logger.LogWarning($"Donor with id {id} not found.");
                throw new Exception($"Donor with id {id} not found.");
            }

            var donorDto = new DonorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.EMail,
            };

            await _cache.SetAsync(cacheKey, donorDto, TimeSpan.FromMinutes(30));

            return donorDto;
        }


        public async Task<DonorDto?> CreateNewDonor(CreateDonorDto donorDto)
        {
            _logger.LogInformation($"Creating new donor with email {donorDto.Email}");
            var donor = new Donor
            {
                Name = donorDto.Name,
                EMail = donorDto.Email,
                GiftsList = new List<Gift>()
            };
            var exist = await _repository.GetDonorByEmail(donor.EMail);
            _logger.LogInformation($"Checking if donor with email {donorDto.Email} already exists");
            if (exist != null)
            {
                _logger.LogWarning($"Donor with email {donorDto.Email} already exists.");   
                throw new Exception("Donor with this email already exists.");
            }
            var d = await _repository.CreateNewDonor(donor);
            if (d == null)
            {
                _logger.LogError("Donor could not be created.");
                throw new Exception("Donor could not be created.");
            }

            // Invalidate cache for all donors
            await _cache.RemoveAsync("donors:all");

            return new DonorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.EMail
            };
        }

        public async Task<DonorDto?> UpdateDonor(DonorDto donorDto)
        {
            _logger.LogInformation($"start Updating donor with id {donorDto.Id}");
            Donor donor = new Donor
            {
                Id = donorDto.Id,
                Name = donorDto.Name,
                EMail = donorDto.Email,
            };
            _logger.LogInformation($"try Updating donor with id {donorDto.Id}");
            var d = await _repository.UpdateDonor(donor);
            
            if (d == null)
            {
                _logger.LogError("Donor could not be updated.");
                throw new Exception("Donor could not be updated.");
            }

            // Invalidate cache for the specific donor and all donors
            await _cache.RemoveAsync($"donor:{donorDto.Id}");
            await _cache.RemoveAsync("donors:all");

            return new DonorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.EMail
            };
        }

        public async Task<IEnumerable<DonorDto>> GetAllDonors()
        {
            var cacheKey = "donors:all";
            var cachedDonors = await _cache.GetAsync<List<DonorDto>>(cacheKey);
            if (cachedDonors != null)
            {
                _logger.LogInformation("All donors retrieved from cache");
                return cachedDonors;
            }

            var donors = await _repository.GetAllDonors();
            var donorDtos = donors.Select(d => new DonorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.EMail,
            }).ToList();

            await _cache.SetAsync(cacheKey, donorDtos, TimeSpan.FromMinutes(30));

            return donorDtos;
        }

        public async Task<DonorDto?> DeleteDonor(int id)
        {
            _logger.LogInformation($"start Deleting donor with id {id}");
            var d = await _repository.DeleteDonor(id);
            if (d == null)
            {
                _logger.LogError($"Donor with id {id} not found.");
                throw new Exception($"Donor with id {id} not found.");
            }

            // Invalidate cache for the specific donor and all donors
            await _cache.RemoveAsync($"donor:{id}");
            await _cache.RemoveAsync("donors:all");

            return new DonorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.EMail,
            };
        }
        public async Task<IEnumerable<GiftDto>> GetDonorGiftList(int id)
        {
            _logger.LogInformation($"Fetching gift list for donor with id {id}");
            var gifts = await _repository.GetDonorGiftList(id);
            _logger.LogInformation($"Found {gifts.Count()} gifts for donor with id {id}");
            return gifts.Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Cost = g.Cost,
                Picture = g.Picture,
                CategoryId = g.CategoryId,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName
            }).ToList();
        }
    }
}