using BsdFinalProject.Models;

namespace BsdFinalProject.IRepositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category?> GetCategoryById(int id);
    }
}