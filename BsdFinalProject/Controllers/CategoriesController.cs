using BsdFinalProject.Data;
using BsdFinalProject.DTOs;
using BsdFinalProject.Models;
using BsdFinalProject.Services;
using FinalProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BsdFinalProject.IServices;

namespace BsdFinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly SaleContext _context;
        private readonly ICategoryService _CategoryService;
        private readonly ILogger<CategoriesController> _logger;
        //public BasketsController(SaleContext context) => _context = context;

        public CategoriesController(ICategoryService categoryService, SaleContext context, ILogger<CategoriesController> logger)
        {
            _CategoryService = categoryService;
            _context = context;
            _logger = logger;

        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDto?>>> GetAllCategories()
        {
            var categories = await _CategoryService.GetAllCategories();
            return Ok(categories);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            try
            {
                var category = await _CategoryService.GetCategoryById(id);
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching category with ID: {CategoryId}", id);
                return NotFound(new { message = ex.Message });
            }
        }
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        //{
        //    var list = await _context.Category
        //        .Select(c => new CategoryDto {
        //            Id = c.Id,
        //            Name = c.Name
        //        })
        //        .ToListAsync();
        //    return Ok(list);
        //}

        //[HttpGet("{id:int}")]
        //public async Task<ActionResult<CategoryDto>> GetById(int id)
        //{
        //    var c = await _context.Category.FindAsync(id);
        //    if (c == null) return NotFound();
        //    return Ok(new CategoryDto { Id = c.Id, Name = c.Name });
        //}

        //[HttpPost]
        //public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto create)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    var category = new Category { Name = create.Name };
        //    _context.Category.Add(category);
        //    await _context.SaveChangesAsync();
        //    return CreatedAtAction(nameof(GetById), new { id = category.Id }, new CategoryDto { Id = category.Id, Name = category.Name });
        //}

        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> Update(int id, CreateCategoryDto update)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    var category = await _context.Category.FindAsync(id);
        //    if (category == null) return NotFound();
        //    category.Name = update.Name;
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpDelete("{id:int}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var category = await _context.Category.FindAsync(id);
        //    if (category == null) return NotFound();
        //    _context.Category.Remove(category);
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}
    }
}