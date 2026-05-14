using BsdFinalProject.Models;

namespace BsdFinalProject.IRepositories
{
    public interface IWinnerRepository
    {
        Task<Winner?> CreateNewWinner(Winner winner);
        Task<IEnumerable<Winner?>> DeleteAllWinners();
        Task<IEnumerable<Winner?>> GetAllWinners();
        Task<IEnumerable<int?>> GetUsersIdForGift(int Giftid);
        Task<Winner?> GetWinnerByGiftId(int giftid);
    }
}