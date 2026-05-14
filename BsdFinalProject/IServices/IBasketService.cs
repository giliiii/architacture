using BsdFinalProject.DTOs;

namespace BsdFinalProject.IServices
{
    public interface IBasketService
    {
        Task<CreateBasketDto> CreateNewBasket(CreateBasketDto basket);
        Task DeleteAllBasket(int id);
        Task<BasketDto> DeleteOneBasket(int id);
        Task<List<BasketDto>> GetAllMyBasket(int Id);
    }
}