using BsdFinalProject.Data;
using BsdFinalProject.IRepositories;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SaleContextFactory _saleContextFactory;
        private Lazy<SaleContext> _lazyContext;

        public CategoryRepository(SaleContextFactory saleContextFactory)
        {
            _saleContextFactory = saleContextFactory;
            _lazyContext = new Lazy<SaleContext>(() => _saleContextFactory.CreateContext());
        }

        private SaleContext _context => _lazyContext.Value;

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Category.ToListAsync();
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            var c = await _context.Category.FindAsync(id);
            return c == null ? null : c;
        }
    }
}