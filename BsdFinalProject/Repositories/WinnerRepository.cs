using BsdFinalProject.Data;
using BsdFinalProject.IRepositories;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Repositories
{
    public class WinnerRepository : IWinnerRepository
    {
        private readonly SaleContextFactory _saleContextFactory;
        private Lazy<SaleContext> _lazyContext;

        public WinnerRepository(SaleContextFactory saleContextFactory)
        {
            _saleContextFactory = saleContextFactory;
            _lazyContext = new Lazy<SaleContext>(() => _saleContextFactory.CreateContext());
        }


        private SaleContext _context => _lazyContext.Value;
        public async Task<Winner?> CreateNewWinner(Winner winner)
        {
            _context.Winner.Add(winner);
            await _context.SaveChangesAsync();
            return winner == null ? null : winner;
        }

        public async Task<IEnumerable<Winner?>> GetAllWinners()
        {
            return await _context.Winner.ToListAsync();
        }
        public async Task<Winner?> GetWinnerByGiftId(int giftid)
        {
            return await _context.Winner.FirstOrDefaultAsync(w => w.IdGift == giftid);
        }

        public async Task<IEnumerable<Winner?>> DeleteAllWinners()
        {
            var winners = await _context.Winner.ToListAsync();
            if (winners == null || winners.Count == 0) return null;
            _context.Winner.RemoveRange(winners);
            await _context.SaveChangesAsync();
            return winners;
        }

        public async Task<IEnumerable<int?>> GetUsersIdForGift(int Giftid)//זה id של winner
        {
            var gift = await _context.Gift.FindAsync(Giftid);
            List<int?> userIds = new List<int?>();
            foreach (var card in gift.CardsList)
            {
                userIds.Add(card.UserId);
            }
            return userIds == null ? null : userIds;
        }
    }
}