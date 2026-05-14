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
    public class WinnerService : IWinnerService
    {
        private readonly IWinnerRepository _repository;
        private readonly IGiftRepository _giftRepository;
        private readonly ILogger<WinnerService> _logger;
        private readonly ICacheService _cache;

        public WinnerService(IWinnerRepository repository, IGiftRepository giftRepository, ILogger<WinnerService> logger, ICacheService cache)
        {
            _repository = repository;
            _giftRepository = giftRepository;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<WinnerDto?>> GetAllWinners()
        {
            _logger.LogInformation("start Fetching all winners from the repository.");

            var cacheKey = "winners:all";
            var cachedWinners = await _cache.GetAsync<List<WinnerDto>>(cacheKey);
            if (cachedWinners != null)
            {
                _logger.LogInformation("All winners retrieved from cache");
                return cachedWinners;
            }

            var winners = await _repository.GetAllWinners();
            if (winners == null)
            {
                _logger.LogWarning("No winners found in the repository.");
                throw new Exception("No winners found.");
            }
            IEnumerable<WinnerDto?> winnerDtos = winners.Select(c => new WinnerDto
            {
                Id = c.Id,
                IdGift = c.IdGift,
                IdUser = c.IdUser
            }).ToList();

            await _cache.SetAsync(cacheKey, winnerDtos, TimeSpan.FromMinutes(30));

            return winnerDtos;
        }

        public async Task<WinnerDto?> CreateNewWinner(int giftId)
        {
            _logger.LogInformation("start Creating a new winner for giftId: {giftId}", giftId);
            var existingWinner = await _repository.GetWinnerByGiftId(giftId);
            _logger.LogInformation("Checking for existing winner for giftId: {giftId}", giftId);
            if (existingWinner != null)
            {
                _logger.LogWarning("Winner already exists for giftId: {giftId}", giftId);
                throw new Exception("Winner for this gift already exists.");
            }
            _logger.LogInformation("Fetching user IDs for giftId: {giftId}", giftId);
            List<int?> userIds = (await _repository.GetUsersIdForGift(giftId)).ToList();
            if (userIds == null || userIds.Count == 0)
            {
                _logger.LogWarning("No users found for giftId: {giftId}", giftId);
                throw new Exception("No users found for this gift.");
            }
            Random rand = new Random();
            int randomIndex = rand.Next(userIds.Count());
            int? randomUserId = userIds.ElementAt(randomIndex);
            Winner winner = new Winner
            {
                IdGift = giftId,
                IdUser = randomUserId.Value
            };
            _logger.LogInformation("Creating winner for userId: {userId} and giftId: {giftId}", randomUserId, giftId);
            var createdWinner = await _repository.CreateNewWinner(winner);
            if (createdWinner == null)
            {
                _logger.LogError("Failed to create winner for giftId: {giftId}", giftId);
                throw new Exception("Failed to create winner.");
            }
            WinnerDto winnerDto = new WinnerDto
            {
                Id = createdWinner.Id,
                IdGift = createdWinner.IdGift,
                IdUser = createdWinner.IdUser
            };

            // Invalidate cache for all winners
            await _cache.RemoveAsync("winners:all");

            return winnerDto;
        }
        public async Task<bool> DeleteAllWinners()
        {
            _logger.LogInformation("start Deleting all winners and resetting associated gifts.");
            var winners = await _repository.GetAllWinners();
            _logger.LogInformation("Fetched all winners for deletion.");
            if (winners == null)
            {
                _logger.LogWarning("No winners found to delete.");
                throw new Exception("No winners to delete.");
            }
            foreach (var winner in winners)
            {
                var gift = winner.Gift;
                gift.WinnerName = "";
                var g = await _giftRepository.UpdateGift(gift);
                _logger.LogInformation("Reset winner for giftId: {giftId}", gift.Id);
                if (g == null)
                {
                    _logger.LogError("Failed to update gift with id {GiftId} while deleting winners.", gift.Id);
                    throw new Exception("Failed to update gift while deleting winners.");
                }
            }
            _logger.LogInformation("All associated gifts have been reset.");
            var deletedWinners = await _repository.DeleteAllWinners();
            _logger.LogInformation("All winners have been deleted.");
            if (deletedWinners == null)
            {
                _logger.LogWarning("No winners were deleted.");
                throw new Exception("No winners to delete.");
            }

            // Invalidate cache for all winners
            await _cache.RemoveAsync("winners:all");

            return true;
        }
    }
}