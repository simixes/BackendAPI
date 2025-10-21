using BackendAPI.Domain.Entities;
using BackendAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendAPI.Contracts.Products;
using BackendAPI.Common;

namespace BackendAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllProducts(
    [FromQuery] int? page,
    [FromQuery] int? pageSize,
    [FromQuery] string? slug = null)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(slug))
        {
            query = query.Where(p => p.UrlSlug == slug);
        }

      
        if (page == null || pageSize == null)
        {
            var allProducts = await query.ToListAsync();
            return Ok(allProducts);
        }

        
        var totalCount = await query.CountAsync();

        var products = await query
            .OrderBy(p => p.Id)
            .Skip((page.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync();

        var response = new
        {
            totalCount,
            currentPage = page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize.Value),
            products
        };

        return Ok(response);
    }



    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProductByID(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductListDto>> CreateProduct([FromBody] ProductCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Product name is required.");

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Image = dto.Image,
            UrlSlug = dto.Name.ToUrlSlug()
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var result = new ProductListDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Image = product.Image,
            UrlSlug = product.UrlSlug
        };

        return CreatedAtAction(nameof(GetProductByID), new { id = product.Id }, result);
    }

    [HttpDelete("{id:int}")]

    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound(); 

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent(); 
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto dto)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            product.Name = dto.Name;
            product.UrlSlug = dto.Name.ToUrlSlug();
        }

        if (!string.IsNullOrWhiteSpace(dto.Description))
            product.Description = dto.Description;

        if (dto.Price.HasValue)
            product.Price = dto.Price.Value;

        if (!string.IsNullOrWhiteSpace(dto.Image))
            product.Image = dto.Image;

        await _context.SaveChangesAsync();

        return NoContent();
    }


}
