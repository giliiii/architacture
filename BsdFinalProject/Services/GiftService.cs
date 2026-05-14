//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;
using BsdFinalProject.IRepositories;
using BsdFinalProject.IServices;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;
using Chocolate.Data;

namespace BsdFinalProject.Services
{
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _repository ;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<GiftService> _logger;
        private readonly ICacheService _cache;

        public GiftService(IGiftRepository repository,ICategoryRepository categoryRepository, ILogger<GiftService> logger, ICacheService cache)    
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _cache = cache;
        }

        public async Task<GiftDto> CreateNewGift(GiftDto giftDto)
        {
            _logger.LogInformation("Creating a new gift");
            Gift gift = new Gift
            {
                Name = giftDto.Name,
                Description = giftDto.Description,
                Cost = giftDto.Cost,
                Picture = giftDto.Picture,
                CategoryId = giftDto.CategoryId,
                DonorId = giftDto.DonorId,
            };
            try
            {
                var g = await _repository.CreateNewGift(gift);
                _logger.LogInformation("Gift created successfully with id: {GiftId}", g.Id);

                // Invalidate cache for all gifts
                await _cache.RemoveAsync("gifts:all");

                return new GiftDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Cost = g.Cost,
                    Picture = g.Picture,
                    CategoryId = g.CategoryId,
                    DonorId = g.DonorId,
                    WinnerName = g.WinnerName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new gift");
                throw new Exception("Gift could not be created: " + ex.Message);
            }
       
        }
        public async Task<GiftDto?> GetGiftById(int id) { 
        
            _logger.LogInformation("start Retrieving gift with id: {GiftId}", id);

            var cacheKey = $"gift:{id}";
            var cachedGift = await _cache.GetAsync<GiftDto>(cacheKey);
            if (cachedGift != null)
            {
                _logger.LogInformation("Gift retrieved from cache with id: {GiftId}", id);
                return cachedGift;
            }

            var g = await _repository.GetGiftById(id);
            if (g == null)
            {
                _logger.LogWarning("Gift with id: {GiftId} not found", id);
                throw new Exception($"Gift with id {id} not found.");
            }

            var giftDto = new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Cost = g.Cost,
                Picture = g.Picture,
                CategoryId = g.CategoryId,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName
            };

            await _cache.SetAsync(cacheKey, giftDto, TimeSpan.FromMinutes(30));

            return giftDto;
        }
        public async Task<List<GiftDto>> GetAllGifts()
        {
            _logger.LogInformation("start Retrieving all gifts");

            var cacheKey = "gifts:all";
            var cachedGifts = await _cache.GetAsync<List<GiftDto>>(cacheKey);
            if (cachedGifts != null)
            {
                _logger.LogInformation("All gifts retrieved from cache");
                return cachedGifts;
            }

            var gifts = await _repository.GetAllGifts();
            _logger.LogInformation("Retrieved gifts");

            var giftDtos = gifts.Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Cost = g.Cost,
                Picture = g.Picture,
                CategoryId = g.CategoryId,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName
            }).ToList();

            await _cache.SetAsync(cacheKey, giftDtos, TimeSpan.FromMinutes(30));

            return giftDtos;
        }
        public async Task<GiftDto?> UpdateGift(GiftDto giftDto)
        {
            _logger.LogInformation("start Updating gift with id {GiftId}", giftDto.Id);
            var existingGift = await _repository.GetGiftById(giftDto.Id);
            _logger.LogInformation("Retrieved existing gift for update");
            if (existingGift == null)
            {
                _logger.LogWarning("Gift with id {GiftId} not found", giftDto.Id);
                throw new Exception($"Gift with id {giftDto.Id} not found.");
            }
            existingGift.Name = giftDto.Name;
            existingGift.Description = giftDto.Description;
            existingGift.Cost = giftDto.Cost;
            existingGift.Picture = giftDto.Picture;
            existingGift.CategoryId = giftDto.CategoryId;
            existingGift.DonorId = giftDto.DonorId;
            existingGift.WinnerName = giftDto.WinnerName;
            _logger.LogInformation("Updating gift in repository");
            var updatedGift = await _repository.UpdateGift(existingGift);
            if (updatedGift == null)
            {
                _logger.LogError("Failed to update gift with id {GiftId}", giftDto.Id);
                throw new Exception("Gift could not be updated.");
            }

            // Invalidate cache for the specific gift and all gifts
            await _cache.RemoveAsync($"gift:{giftDto.Id}");
            await _cache.RemoveAsync("gifts:all");

            return giftDto;
        }
        public async Task<bool> DeleteGift(int id)
        {
            _logger.LogInformation("start Deleting gift with id {GiftId}", id);
            var deleteGift = await _repository.DeleteGift(id);
            _logger.LogInformation("Retrieved gift for deletion");

            if (deleteGift)
            {
                // Invalidate cache for the specific gift and all gifts
                await _cache.RemoveAsync($"gift:{id}");
                await _cache.RemoveAsync("gifts:all");
            }

            return deleteGift != null;
        }
        public async Task<List<GiftDto>> GetGiftsByCategoryId(int categoryId)
        {
            _logger.LogInformation("start Retrieving gifts for category id {CategoryId}", categoryId);
            var gifts = await _repository.GetGiftsByCategory(categoryId);
            if (gifts == null)
            {
                throw new Exception($"No gifts found for category id {categoryId}.");
                _logger.LogWarning("No gifts found for category id {CategoryId}", categoryId);
            }
                return gifts.Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Cost = g.Cost,
                Picture = g.Picture,
                CategoryId = g.CategoryId,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName
            }).ToList();
        }
        public async Task<List<GiftDto>> GetGiftsByCost(int price1, int price2)
        {
            _logger.LogInformation("start Retrieving gifts with cost between {Price1} and {Price2}", price1, price2);
            if (price1 < 0 || price2 < 0)
            {
                _logger.LogWarning("Invalid price values: {Price1}, {Price2}", price1, price2);
                throw new Exception("Price values must be non-negative.");
            }

            if (price1 > price2)
            {
                var temp = price1;
                price1 = price2;
                price2 = temp;
            }
            _logger.LogInformation("Fetching gifts from repository");
            var gifts = await _repository.GetGiftByCost(price1, price2);
            return gifts.Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Cost = g.Cost,
                Picture = g.Picture,
                CategoryId = g.CategoryId,
                DonorId = g.DonorId,
                WinnerName = g.WinnerName
            }).ToList();
        }
    }
}