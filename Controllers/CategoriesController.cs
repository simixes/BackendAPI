using BackendAPI.Contracts.Categories;
using BackendAPI.Common;
using BackendAPI.Domain.Entities;
using BackendAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BackendAPI.Controllers;

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
    public async Task<ActionResult> GetCategoryById(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound();

        var result = new
        {
            category.Id,
            category.Name,
            category.Image,
            category.Slug,
            Products = category.Products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Image,
                p.UrlSlug
            })
        };

        return Ok(result);
    }


    [Authorize]
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

    [Authorize]
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

    [Authorize]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return NotFound();

        
        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            category.Name = dto.Name;
            category.Slug = dto.Name.ToUrlSlug();
        }

        if (!string.IsNullOrWhiteSpace(dto.Image))
            category.Image = dto.Image;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpPut("{categoryId:int}/products/{productId:int}")]
    public async Task<IActionResult> AddProductToCategory(int categoryId, int productId)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
            return NotFound("Category not found");

        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return NotFound("Product not found");

        if (category.Products.Any(p => p.Id == productId))
            return BadRequest("Product already exists in this category");

        category.Products.Add(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{categoryId:int}/products/{productId:int}")]
    public async Task<IActionResult> RemoveProductFromCategory(int categoryId, int productId)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
            return NotFound("Category not found");

        var product = category.Products.FirstOrDefault(p => p.Id == productId);
        if (product == null)
            return NotFound("Product not found in this category");

        category.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}

