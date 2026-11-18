using CineNord.Rest.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL (rest_db) og Redis (blacklist til JWT-revocation)
builder.Services.AddDbContext<RestDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: AddAuthentication().AddJwtBearer(...) + middleware der tjekker blacklist:{jti} i Redis

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Tjek at databaserne svarer: GET http://localhost:5101/health
app.MapGet("/health", async (RestDbContext db, IConnectionMultiplexer redis) => new
{
    postgres = await db.Database.CanConnectAsync(),
    redis = redis.IsConnected
});

app.Run();
