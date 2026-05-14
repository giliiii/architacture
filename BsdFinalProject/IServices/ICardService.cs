//using BsdFinalProject.IRepositories;
//using BsdFinalProject.IServices;
using BsdFinalProject.DTOs;
using BsdFinalProject.Models;

namespace BsdFinalProject.IServices
{
    public interface ICardService
    {
        Task<IEnumerable<CardDto?>> CreateNewcCards(List<BasketDto> baskets);
        Task<IEnumerable<GiftDtoWithSum?>> GetAllMyCard(int Id);
        Task<Card?> GetCardById(int id);
        Task<IEnumerable<CardWithBuyerDto?>> GetAllPurchasesOrderedByMostPurchasedGift();
        Task<IEnumerable<CardWithBuyerDto>> GetAllCardsWithBuyers();
        Task<IEnumerable<CardWithBuyerDto>> GetAllPurchasesOrderedByCost();
    }
}