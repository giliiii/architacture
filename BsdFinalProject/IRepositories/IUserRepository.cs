using BsdFinalProject.Models;

namespace BsdFinalProject.IRepositories
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<User?> GetByEmail(string email);
    }
}