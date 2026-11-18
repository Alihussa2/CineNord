using CineNord.Sse.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SseDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// GET http://localhost:5106/v1/screenings/7/seat-events
app.MapGet("/v1/screenings/{id:int}/seat-events", async (int id, HttpContext ctx, CancellationToken ct) =>
{
    ctx.Response.Headers.ContentType = "text/event-stream";
    ctx.Response.Headers.CacheControl = "no-cache";

    var tick = 0;
    try
    {
        while (!ct.IsCancellationRequested)
        {
            // TODO: laes aendringer i SeatHolds fra databasen i stedet for denne heartbeat
            await ctx.Response.WriteAsync($"event: heartbeat\ndata: {{\"screeningId\":{id},\"tick\":{tick++}}}\n\n", ct);
            await ctx.Response.Body.FlushAsync(ct);
            await Task.Delay(1000, ct);
        }
    }
    catch (OperationCanceledException)
    {
        // klienten lukkede forbindelsen
    }
});

app.MapGet("/health", async (SseDbContext db) => new { postgres = await db.Database.CanConnectAsync() });

app.Run();
