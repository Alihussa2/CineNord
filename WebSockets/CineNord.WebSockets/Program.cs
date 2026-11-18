using System.Net.WebSockets;
using CineNord.WebSockets.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WsDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

app.UseWebSockets();

// ws://localhost:5105/ws/seating
app.Map("/ws/seating", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    using var socket = await context.WebSockets.AcceptWebSocketAsync();
    var buffer = new byte[4096];

    while (socket.State == WebSocketState.Open)
    {
        var result = await socket.ReceiveAsync(buffer, context.RequestAborted);
        if (result.MessageType == WebSocketMessageType.Close)
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None);
            break;
        }

        // TODO: parse JSON ("reserveSeat" / "seatAction") og svar som gRPC-operationerne.
        // Indtil videre sendes beskeden bare tilbage (echo).
        await socket.SendAsync(buffer.AsMemory(0, result.Count), WebSocketMessageType.Text, true, context.RequestAborted);
    }
});

app.MapGet("/health", async (WsDbContext db) => new { postgres = await db.Database.CanConnectAsync() });

app.Run();
