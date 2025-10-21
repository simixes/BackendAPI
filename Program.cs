using BackendAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Lägg till kontrollers
builder.Services.AddControllers();

// Lägg till Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FreakyFashion API",
        Version = "v1",
        Description = "Ett API för produkter och kategorier"
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Aktivera Swagger endast i utvecklingsläge
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreakyFashion API v1");
        c.RoutePrefix = string.Empty; // öppnar Swagger direkt på root URL
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

