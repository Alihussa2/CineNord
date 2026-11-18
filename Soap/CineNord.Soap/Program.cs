using CineNord.Soap.Contracts;
using CineNord.Soap.Data;
using CineNord.Soap.Services;
using Microsoft.EntityFrameworkCore;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SoapDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddSoapCore();
builder.Services.AddScoped<IBookingService, BookingService>();

var app = builder.Build();

// WSDL: http://localhost:5102/BookingService.asmx?wsdl
// Cast fordi WebApplication har to UseSoapEndpoint-overloads (ellers "ambiguous call")
((IEndpointRouteBuilder)app).UseSoapEndpoint<IBookingService>(
    "/BookingService.asmx", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);

app.MapGet("/health", async (SoapDbContext db) => new { postgres = await db.Database.CanConnectAsync() });

app.Run();
