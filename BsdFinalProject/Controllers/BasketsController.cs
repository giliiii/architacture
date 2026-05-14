
using BsdFinalProject.Controllers;
using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.Models;
using BsdFinalProject.Services;
using FinalProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinalProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BasketsController : ControllerBase
    {
        private readonly SaleContext _context;
        private readonly BasketService _BasketService;
        private readonly ILogger<BasketsController> _logger;
        //public BasketsController(SaleContext context) => _context = context;

        public BasketsController(BasketService basketService, SaleContext context, ILogger<BasketsController> logger)
        {
            _BasketService = basketService;
            _context = context;
            _logger = logger;
        }

        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<BasketDto>>> GetAll()
        {
            var list = await _context.Basket
                .Select(b => new BasketDto {
                    Id = b.Id,
                    UserId = b.UserId,
                    GiftId = b.GiftId
                })
                .ToListAsync();
            return Ok(list);
        }*/

        [HttpGet]
        public async Task<ActionResult<List<BasketDto>>> GetAllMyBasket()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdValue == null)
            {
                _logger.LogWarning("Unauthorized access attempt to GetAllMyBasket.");
                return Unauthorized();             
            }

            int userId = int.Parse(userIdValue);

            var baskets = await _BasketService.GetAllMyBasket(userId);

            return Ok(baskets); 
        }
        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateNewBasket(CreateBasketDto b)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt to CreateNewBasket.");
                return Unauthorized();
            }
            try
            {
                var basket = await _BasketService.CreateNewBasket(b);
                return Ok(basket);
            }
            catch (ArgumentException ex)
            { 
             _logger.LogWarning(ex, "Invalid argument provided.");
             return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new basket.");
                return NotFound(new { message = ex.Message });             
            }
        }



        [HttpDelete("{id:int}")]
        public async Task<ActionResult<BasketDto>> DeleteOneBasket(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt to DeleteOneBasket.");
                return Unauthorized();
            }
            try
            {
                var basket = await _BasketService.DeleteOneBasket(id);
                return Ok(basket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a basket with ID {BasketId}.", id);
                return NotFound(new { message = ex.Message });              
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAllBasket(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt to DeleteAllBasket.");
                return Unauthorized();            
            }
            int newUserId = int.Parse(userId);
            try
            {
                await _BasketService.DeleteAllBasket(newUserId);
                return Ok(new { message = "All baskets deleted successfully" });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting all baskets for user ID {UserId}.", newUserId);
                return NotFound(new { message = ex.Message });
                
            }

        }
    }
}

