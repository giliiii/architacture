using BsdFinalProject.Models;

namespace BsdFinalProject.IRepositories
{
    public interface IDonorRepository
    {
        Task<Donor> CreateNewDonor(Donor donor);
        Task<Donor?> DeleteDonor(int id);
        Task<IEnumerable<Donor>> GetAllDonors();
        Task<Donor?> GetDonorByEmail(string email);
        Task<Donor?> GetDonorById(int id);
        Task<IEnumerable<Gift?>> GetDonorGiftList(int id);
        Task<Donor?> UpdateDonor(Donor donor);
    }
}