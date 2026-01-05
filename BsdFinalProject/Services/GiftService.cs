//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.Models;
using BsdFinalProject.Repositories;
using BsdFinalProject.DTOs;

namespace BsdFinalProject.Services
{
    public class GiftService
    {
        private readonly GiftRepository _repository = new();

        public async Task<GiftDto> CreateNewGift(GiftDto giftDto)
        {
            Gift gift = new Gift
            {
                Name = giftDto.Name,
                Description = giftDto.Description,
                Cost = giftDto.Cost,
                Picture = giftDto.Picture,
                CategoryId = giftDto.CategoryId,
                DonorId = giftDto.DonorId,
                WinnerName = giftDto.WinnerName
            };

            var g = await _repository.CreateNewGift(gift);
            return g == null ? null : giftDto;
        }
        public async Task<GiftDto?> GetGiftById(int id)
        {
            var g = await _repository.GetGiftById(id);
            if (g == null) return null;
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
        public async Task<List<GiftDto>> GetAllGifts()
        {
            var gifts = await _repository.GetAllGifts();
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
        public async Task<GiftDto?> UpdateGift(GiftDto giftDto)
        {
            var existingGift = await _repository.GetGiftById(giftDto.Id);
            if (existingGift == null) return null;
            existingGift.Name = giftDto.Name;
            existingGift.Description = giftDto.Description;
            existingGift.Cost = giftDto.Cost;
            existingGift.Picture = giftDto.Picture;
            existingGift.CategoryId = giftDto.CategoryId;
            existingGift.DonorId = giftDto.DonorId;
            existingGift.WinnerName = giftDto.WinnerName;
            var updatedGift = await _repository.UpdateGift(existingGift);
            if (updatedGift == null) return null;
            return giftDto;

        }
        //public async Task<bool> DeleteGift(int id)
        //{
        //    return await _repository.DeleteGift(id);
        //}
        //public async Task<List<GiftDto>> GetGiftsByCategoryId(int categoryId)
        //{
        //    var gifts = await _repository.GetGiftsByCategoryId(categoryId);
        //    return gifts.Select(g => new GiftDto
        //    {
        //        Id = g.Id,
        //        Name = g.Name,
        //        Description = g.Description,
        //        Cost = g.Cost,
        //        Picture = g.Picture,
        //        CategoryId = g.CategoryId,
        //        DonorId = g.DonorId,
        //        WinnerName = g.WinnerName
        //    }).ToList();
        //}
    }
}