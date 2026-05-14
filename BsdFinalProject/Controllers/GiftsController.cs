using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.IServices;
using BsdFinalProject.Models;
using BsdFinalProject.Services;
using FinalProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BsdFinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiftsController : ControllerBase
    {
        private readonly SaleContext _context;
        private readonly IGiftService _GiftService;
        private readonly ILogger<GiftsController> _logger;
        //public BasketsController(SaleContext context) => _context = context;

        public GiftsController(IGiftService giftService, SaleContext context, ILogger<GiftsController> logger)
        {
            _GiftService = giftService;
            _context = context;
            _logger = logger;

        }

        [HttpGet]
        public async Task<ActionResult<List<GiftDto>>> GetAllGifts()
        {
            var gifts = await _GiftService.GetAllGifts();
            return Ok(gifts);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GiftDto>> GetGiftById(int id)
        {
            try
            {
                var gift = await _GiftService.GetGiftById(id);
                return Ok(gift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving gift with ID {GiftId}", id);
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<GiftDto>> CreateNewGift(GiftDto giftDto)
        {
            try
            {
                var createdGift = await _GiftService.CreateNewGift(giftDto);
                return CreatedAtAction(nameof(GetGiftById), new { id = createdGift.Id }, createdGift);
            }
            catch(ArgumentException ex)
            {
                _logger.LogError(ex, "Validation error while creating a new gift.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating a new gift.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<GiftDto>> UpdateGift(int id, GiftDto giftDto)
        {
            if (id != giftDto.Id)
            {
                _logger.LogWarning("ID mismatch: URL ID {UrlId} does not match body ID {BodyId}", id, giftDto.Id);
                return BadRequest(new { message = "ID mismatch." });
            }
            try
            {
                var updatedGift = await _GiftService.UpdateGift(giftDto);
                return Ok(updatedGift);
            }
            catch(ArgumentException ex)
            {
                _logger.LogError(ex, "Validation error while updating gift with ID {GiftId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating gift with ID {GiftId}", id);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<GiftDto>> DeleteGift(int id)
        {  
            var deletedGift = await _GiftService.DeleteGift(id);
            return Ok(deletedGift);
        }

        [HttpGet("category/{categoryId:int}")]
        public async Task<ActionResult<List<GiftDto>>> GetGiftsByCategory(int categoryId)
        {
            try
            {
                var gifts = await _GiftService.GetGiftsByCategoryId(categoryId);
                return Ok(gifts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving gifts for category ID {CategoryId}", categoryId);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("cost/{Price1:int}/{Price2:int}")]
        public async Task<ActionResult<List<GiftDto>>> GetGiftsByCost(int Price1, int Price2)
        {
            try
            {
                var gifts = await _GiftService.GetGiftsByCost(Price1, Price2);
                return Ok(gifts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving gifts with cost between {Price1} and {Price2}", Price1, Price2);
                return NotFound(new { message = ex.Message });
            }
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<GiftDto>>> GetAll()
        //{
        //    var list = await _context.Gift
        //        .Select(g => new GiftDto
        //        {
        //            Id = g.Id,
        //            Name = g.Name,
        //            Description = g.Description,
        //            Cost = g.Cost,
        //            Picture = g.Picture,
        //            CategoryId = g.CategoryId,
        //            DonorId = g.DonorId,
        //            WinnerName = g.WinnerName
        //        })
        //        .ToListAsync();
        //    return Ok(list);
        //}

        //[HttpGet("{id:int}")]
        //public async Task<ActionResult<GiftDto>> GetById(int id)
        //{
        //    var g = await _context.Gift.FindAsync(id);
        //    if (g == null) return NotFound();
        //    var dto = new GiftDto
        //    {
        //        Id = g.Id,
        //        Name = g.Name,
        //        Description = g.Description,
        //        Cost = g.Cost,
        //        Picture = g.Picture,
        //        CategoryId = g.CategoryId,
        //        DonorId = g.DonorId,
        //        WinnerName = g.WinnerName
        //    };
        //    return Ok(dto);
        //}

        //[HttpPost]
        //public async Task<ActionResult<GiftDto>> Create(CreateGiftDto create)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    var gift = new Gift
        //    {
        //        Name = create.Name,
        //        Description = create.Description,
        //        Cost = create.Cost,
        //        Picture = create.Picture,
        //        CategoryId = create.CategoryId,
        //        DonorId = create.DonorId
        //    };
        //    _context.Gift.Add(gift);
        //    await _context.SaveChangesAsync();
        //    var dto = new GiftDto
        //    {
        //        Id = gift.Id,
        //        Name = gift.Name,
        //        Description = gift.Description,
        //        Cost = gift.Cost,
        //        Picture = gift.Picture,
        //        CategoryId = gift.CategoryId,
        //        DonorId = gift.DonorId,
        //        WinnerName = gift.WinnerName
        //    };
        //    return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        //}

        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> Update(int id, CreateGiftDto update)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    var gift = await _context.Gift.FindAsync(id);
        //    if (gift == null) return NotFound();
        //    gift.Name = update.Name;
        //    gift.Description = update.Description;
        //    gift.Cost = update.Cost;
        //    gift.Picture = update.Picture;
        //    gift.CategoryId = update.CategoryId;
        //    gift.DonorId = update.DonorId;
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpDelete("{id:int}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var gift = await _context.Gift.FindAsync(id);
        //    if (gift == null) return NotFound();
        //    _context.Gift.Remove(gift);
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}
    }
}