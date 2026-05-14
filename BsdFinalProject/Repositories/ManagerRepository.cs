using BsdFinalProject.Data;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BsdFinalProject.Repositories
{
    public class ManagerRepository
    {
        //    private readonly SaleContextFactory _saleContextFactory;
        //    private Lazy<SaleContext> _lazyContext;

        //    public ManagerRepository(SaleContextFactory saleContextFactory)
        //    {
        //        _saleContextFactory = saleContextFactory;
        //        _lazyContext = new Lazy<SaleContext>(() => _saleContextFactory.CreateContext());
        //    }

        //    private SaleContext _context => _lazyContext.Value;

        //    public async Task<Manager?> GetManagerById(int id)
        //    {
        //        return await _context.Manager.FindAsync(id);
        //    }

        //}
    }
}