using BackendAPI.Contracts.Categories;
using BackendAPI.Common;
using BackendAPI.Domain.Entities;
using BackendAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryListDto>>> GetCategories([FromQuery] string? slug)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(slug))
            {
                query = query.Where(c => c.Slug == slug);
            }

            var categories = await query
                .Select(c => new CategoryListDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Image = c.Image,
                    Slug = c.Slug
                })
                .ToListAsync();

            return Ok(categories);
        }

        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryListDto>> GetCategoryById(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound();

            return Ok(new CategoryListDto
            {
                Id = category.Id,
                Name = category.Name,
                Image = category.Image,
                Slug = category.Slug
            });
        }

        
        [HttpPost]
        public async Task<ActionResult<CategoryListDto>> CreateCategory([FromBody] CategoryCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Category name is required.");

            var category = new Category
            {
                Name = dto.Name,
                Image = dto.Image,
                Slug = dto.Name.ToUrlSlug()
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var result = new CategoryListDto
            {
                Id = category.Id,
                Name = category.Name,
                Image = category.Image,
                Slug = category.Slug
            };

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, result);
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

