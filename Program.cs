using Microsoft.EntityFrameworkCore;
//using FreakyFashion.Api.Infrastructure;
using BackendAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Lägg till databaskoppling (EF Core)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
