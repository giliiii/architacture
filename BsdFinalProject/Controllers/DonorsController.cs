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
    public class DonorsController : ControllerBase
    {
        private readonly SaleContext _context;
        private readonly IDonorService _DonorService;
        private readonly ILogger<DonorsController> _logger;
        //public BasketsController(SaleContext context) => _context = context;

        public DonorsController(IDonorService donorService, SaleContext context, ILogger<DonorsController> logger)
        {
            _DonorService = donorService;
            _context = context;
            _logger = logger;

        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<IEnumerable<DonorDto>>> GetAllDonors()
        {
            var donors = await _DonorService.GetAllDonors();
            return Ok(donors);
        }   

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DonorDto>> GetDonorById(int id)
        {
            try
            {
                var donor = await _DonorService.GetDonorById(id);
                return Ok(donor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving donor with ID {DonorId}", id);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<CreateDonorDto>> CreateNewDonor(CreateDonorDto donorDto)
        {
            try
            {
                var createdDonor = await _DonorService.CreateNewDonor(donorDto);
                return Ok(createdDonor);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation error while creating a new donor");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating a new donor");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<DonorDto>> UpdateDonor(DonorDto donorDto)
        {
            try
            {
                var updatedDonor = await _DonorService.UpdateDonor(donorDto);
                return Ok(updatedDonor);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation error while updating donor with ID {DonorId}", donorDto.Id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating donor with ID {DonorId}", donorDto.Id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<DonorDto>> DeleteDonor(int id)
        {
            try
            {
                var deletedDonor = await _DonorService.DeleteDonor(id);
                return Ok(deletedDonor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting donor with ID {DonorId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("{id:int}/gifts")]
        public async Task<ActionResult<IEnumerable<GiftDto>>> GetDonorGiftList(int id)
        {
            try
            {
                var gifts = await _DonorService.GetDonorGiftList(id);
                return Ok(gifts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving gifts for donor with ID {DonorId}", id);
                return NotFound(new { message = ex.Message });
            }
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<DonorDto>>> GetAll()
        //{
        //    var list = await _context.Donor
        //        .Select(d => new DonorDto {
        //            Id = d.Id,
        //            Name = d.Name,
        //            Email = d.EMail
        //        })
        //        .ToListAsync();
        //    return Ok(list);
        //}

        //[HttpGet("{id:int}")]
        //public async Task<ActionResult<DonorDto>> GetById(int id)
        //{
        //    var d = await _context.Donor.FindAsync(id);
        //    if (d == null) return NotFound();
        //    return Ok(new DonorDto { Id = d.Id, Name = d.Name, Email = d.EMail });
        //}

        //[HttpPost]
        //public async Task<ActionResult<DonorDto>> Create(CreateDonorDto create)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    var donor = new Donor { Name = create.Name, EMail = create.Email };
        //    _context.Donor.Add(donor);
        //    await _context.SaveChangesAsync();
        //    return CreatedAtAction(nameof(GetById), new { id = donor.Id }, new DonorDto { Id = donor.Id, Name = donor.Name, Email = donor.EMail });
        //}

        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> Update(int id, CreateDonorDto update)
        //{
}
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    var donor = await _context.Donor.FindAsync(id);
        //    if (donor == null) return NotFound();
        //    donor.Name = update.Name;
        //    donor.EMail = update.Email;
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpDelete("{id:int}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var donor = await _context.Donor.FindAsync(id);
        //    if (donor == null) return NotFound();
        //    _context.Donor.Remove(donor);
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}
    }