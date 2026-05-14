using BsdFinalProject.Data;
using BsdFinalProject.IRepositories;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BsdFinalProject.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SaleContextFactory _saleContextFactory;
        private Lazy<SaleContext> _lazyContext;

        public UserRepository(SaleContextFactory saleContextFactory)
        {
            _saleContextFactory = saleContextFactory;
            _lazyContext = new Lazy<SaleContext>(() => _saleContextFactory.CreateContext());
        }


        private SaleContext _context => _lazyContext.Value;
        public async Task<User?> GetByEmail(string email)
        {
            return await _context.User
                .FirstOrDefaultAsync(u => u.EMail == email);
        }

        public async Task<User> CreateUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}