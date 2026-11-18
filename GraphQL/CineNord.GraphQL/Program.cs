using CineNord.GraphQL;
using CineNord.GraphQL.Data;
using CineNord.GraphQL.Sharding;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Users ligger i graphql_auth, hashede tokens i Redis, kunder og anmeldelser i 3 shards
builder.Services.AddDbContext<AuthDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Auth")));
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));
builder.Services.AddSingleton<ShardRouter>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

// GraphQL: http://localhost:5103/graphql
app.MapGraphQL();

app.MapGet("/health", async (AuthDbContext db, IConnectionMultiplexer redis, ShardRouter router) =>
{
    var shards = new List<bool>();
    foreach (var cs in router.AllShards)
    {
        try { await using var c = new NpgsqlConnection(cs); await c.OpenAsync(); shards.Add(true); }
        catch { shards.Add(false); }
    }
    return new { auth = await db.Database.CanConnectAsync(), redis = redis.IsConnected, shards };
});

app.Run();
