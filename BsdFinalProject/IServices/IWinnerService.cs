//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;

//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;

namespace BsdFinalProject.IServices
{
    public interface IWinnerService
    {
        Task<WinnerDto?> CreateNewWinner(int giftId);
        Task<bool> DeleteAllWinners();
        Task<IEnumerable<WinnerDto?>> GetAllWinners();
    }
}