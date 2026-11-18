using CineNord.Grpc.Data;
using CineNord.Grpc.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GrpcDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

// gRPC: localhost:5104 (HTTP/2). Test i Postman: New -> gRPC -> server reflection
app.MapGrpcService<SeatingGrpcService>();
app.MapGrpcReflectionService();

app.Run();
