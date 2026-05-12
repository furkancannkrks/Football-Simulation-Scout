using Microsoft.EntityFrameworkCore;
using FootballSimulationApi.Models;
using FootballSimulationApi.Services;
using FootballSimulationApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// PostgreSQL (Npgsql) DbContext Registration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection (DI) Registrations
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ISimulationService, SimulationService>();
builder.Services.AddScoped<IPlayerRatingRepository, PlayerRatingRepository>();
builder.Services.AddScoped<IPlayerRatingService, PlayerRatingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Serve frontend files (HTML, CSS, JS) from wwwroot folder:
app.UseDefaultFiles(); // Sets index.html as the default start page
app.UseStaticFiles();  // Exposes static files in wwwroot

app.UseAuthorization();

app.MapControllers();

app.Run();
