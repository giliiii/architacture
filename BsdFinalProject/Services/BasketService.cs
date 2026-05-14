using BsdFinalProject.DTOs;
using BsdFinalProject.IServices;
//using BsdFinalProject.IServices;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;
using BsdFinalProject.Services;
using Chocolate.Data;
using FinalProject.Repositories;
using Microsoft.Extensions.Logging;

namespace FinalProject.Services
{
    public class BasketService : IBasketService
    {
        private readonly BasketRepository _repository;
        private readonly IGiftService _giftService;
        private readonly ILogger<BasketService> _logger;
        private readonly ICacheService _cache;

        public BasketService(ILogger<BasketService> logger, SaleContextFactory saleContextFactory, IGiftService giftService, ICacheService cache)
        {
            _logger = logger;
            _repository = new BasketRepository(saleContextFactory);
            _giftService = giftService;
            _cache = cache;
        }

        public async Task<List<BasketDto>> GetAllMyBasket(int userId)
        {
            _logger.LogInformation("Fetching baskets for user with ID: {UserId}", userId);

            var cacheKey = $"baskets:user:{userId}";
            var cachedBaskets = await _cache.GetAsync<List<BasketDto>>(cacheKey);
            if (cachedBaskets != null)
            {
                _logger.LogInformation("Baskets retrieved from cache for user with ID: {UserId}", userId);
                return cachedBaskets;
            }

            var baskets = await _repository.GetAllMyBasket(userId);
            _logger.LogInformation("Retrieved  baskets for user with ID: {UserId}",userId);

            var basketDtos = baskets.Select(b => new BasketDto
            {
                Id = b.Id,
                UserId = b.UserId,
                GiftId = b.GiftId
            }).ToList();

            await _cache.SetAsync(cacheKey, basketDtos, TimeSpan.FromMinutes(30));

            return basketDtos;
        }
        public async Task<CreateBasketDto> CreateNewBasket(CreateBasketDto basket)
        {
            _logger.LogInformation("Creating new basket for user with ID: {UserId} and Gift ID: {GiftId}", basket.UserId, basket.GiftId);
            try
            {
                var gift = await _giftService.GetGiftById(basket.GiftId);
                _logger.LogInformation("Fetched gift with ID: {GiftId} for basket creation", basket.GiftId);
                if (gift == null)
                {
                    _logger.LogWarning("Gift with ID: {GiftId} not found", basket.GiftId);
                    throw new Exception("Gift not found");
                }

                Basket b = new();
                b.UserId = basket.UserId;
                b.GiftId = basket.GiftId;


                var B = await _repository.CreateNewBasket(b);

                // Invalidate cache for the user's baskets
                var cacheKey = $"baskets:user:{basket.UserId}";
                await _cache.RemoveAsync(cacheKey);

                return basket;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating basket for user with ID: {UserId} and Gift ID: {GiftId}", basket.UserId, basket.GiftId);
                throw new ApplicationException("Failed to create basket", ex);
            }
        }
        public async Task<BasketDto> DeleteOneBasket(int id)
        {
            _logger.LogInformation("start Deleting basket with ID: {BasketId}", id);
            try
            {
                var basket = await _repository.DeleteOneBasket(id);
                if (basket == null)
                {
                    _logger.LogWarning("Basket with ID: {BasketId} not found for deletion", id);
                    throw new Exception("Basket not found");
                }
                var gift = await _giftService.GetGiftById(basket.GiftId);
                if (gift == null)
                {
                    _logger.LogWarning("Gift with ID: {GiftId} not found during basket deletion", basket.GiftId);
                    throw new Exception("Gift not found");
                }
                BasketDto bd = new();
                bd.Id = basket.Id;
                bd.UserId = basket.UserId;
                bd.GiftId = basket.GiftId;

                // Invalidate cache for the user's baskets
                var cacheKey = $"baskets:user:{basket.UserId}";
                await _cache.RemoveAsync(cacheKey);

                return bd;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting basket with ID: {BasketId}", id);
                throw new ApplicationException("Failed to delete basket", ex);
            }
        }
        public async Task DeleteAllBasket(int userId)
        {
            _logger.LogInformation("start Deleting all baskets for user with ID: {UserId}", userId);
            try
            {
                var deletedBaskets = await _repository.DeleteAllBasket(userId);

                if (!deletedBaskets.Any())
                {
                    _logger.LogWarning("No baskets found to delete for user with ID: {UserId}", userId);
                    throw new Exception("No baskets found to delete");
                }

                // Invalidate cache for the user's baskets
                var cacheKey = $"baskets:user:{userId}";
                await _cache.RemoveAsync(cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting all baskets for user with ID: {UserId}", userId);
                throw new ApplicationException("Failed to delete all baskets", ex);
            }
        }

    }
}

