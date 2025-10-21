using Microsoft.EntityFrameworkCore;
using BackendAPI.Domain.Entities;

namespace BackendAPI.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Category> Categories { get; set; }
}
