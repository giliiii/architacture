//using AutoMapper;
using BsdFinalProject.Data;
using BsdFinalProject.IRepositories;
//using BsdFinalProject.IRepository;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
namespace FinalProject.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly SaleContextFactory _saleContextFactory;
        private Lazy<SaleContext> _lazyContext;

        public BasketRepository(SaleContextFactory saleContextFactory)
        {
            _saleContextFactory = saleContextFactory;
            _lazyContext = new Lazy<SaleContext>(() => _saleContextFactory.CreateContext());
        }


        private SaleContext _context => _lazyContext.Value;
        //private readonly IMapper _mapper;


        public async Task<IEnumerable<Basket>> GetAllMyBasket(int Id)
        {
            return await _context.Basket
                .Where(b => b.UserId == Id)
                .ToListAsync();
        }

        public async Task<Basket> CreateNewBasket(Basket basket)
        {
            _context.Basket.Add(basket);
            await _context.SaveChangesAsync();
            return basket;
        }

        public async Task<Basket> DeleteOneBasket(int id)//זה id של basket
        {
            var basket = await _context.Basket.FindAsync(id);
            if (basket == null) return null;

            _context.Basket.Remove(basket);
            await _context.SaveChangesAsync();
            return basket;
        }
        public async Task<List<Basket>> DeleteAllBasket(int userId)
        {
            var baskets = (await GetAllMyBasket(userId)).ToList();
            if (!baskets.Any()) return new List<Basket>();

            _context.Basket.RemoveRange(baskets);
            await _context.SaveChangesAsync();
            return baskets;
        }

    }
}



