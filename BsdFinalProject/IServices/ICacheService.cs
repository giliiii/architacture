using System.Threading.Tasks;

namespace BsdFinalProject.IServices
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task InvalidateByPatternAsync(string pattern);
    }
}