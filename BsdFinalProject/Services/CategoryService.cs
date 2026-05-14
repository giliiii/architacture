//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;
using BsdFinalProject.IServices;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;
using Chocolate.Data;

namespace BsdFinalProject.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly CategoryRepository _repository;
        private readonly ILogger<CategoryService> _logger;
        private readonly ICacheService _cache;

        public CategoryService(ILogger<CategoryService> logger,SaleContextFactory saleContextFactory, ICacheService cache)
        {
            _logger = logger;
            _repository = new CategoryRepository(saleContextFactory);
            _cache = cache;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategories()
        {   
            _logger.LogInformation("Fetching all categories");

            var cacheKey = "categories:all";
            var cachedCategories = await _cache.GetAsync<List<CategoryDto>>(cacheKey);
            if (cachedCategories != null)
            {
                _logger.LogInformation("Categories retrieved from cache");
                return cachedCategories;
            }

            var categories = await _repository.GetAllCategories();
            if (categories == null) return Enumerable.Empty<CategoryDto>();

            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
            }).ToList();

            await _cache.SetAsync(cacheKey, categoryDtos, TimeSpan.FromMinutes(30));

            return categoryDtos;
        }
        

        public async Task<CategoryDto?> GetCategoryById(int id)
        {
            _logger.LogInformation("Fetching category with ID: {CategoryId}", id);

            var cacheKey = $"category:{id}";
            var cachedCategory = await _cache.GetAsync<CategoryDto>(cacheKey);
            if (cachedCategory != null)
            {
                _logger.LogInformation("Category retrieved from cache with ID: {CategoryId}", id);
                return cachedCategory;
            }

            var c = await _repository.GetCategoryById(id);
            if (c == null)
            {
                _logger.LogWarning("Category with ID: {CategoryId} not found", id);
                throw new Exception($"Category with id {id} not found.");
            }

            var categoryDto = new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
            };

            await _cache.SetAsync(cacheKey, categoryDto, TimeSpan.FromMinutes(30));

            return categoryDto;
        }
    }
}