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
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string? slug)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(slug))
        {
            query = query.Where(p => p.UrlSlug == slug);
        }

        var products = await query.ToListAsync();

        return Ok(products);
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

}
