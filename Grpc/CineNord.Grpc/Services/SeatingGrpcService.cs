using CineNord.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace CineNord.Grpc.Services;

public class SeatingGrpcService : SeatingService.SeatingServiceBase
{
    public override Task<ReserveSeatResponse> ReserveSeat(ReserveSeatRequest request, ServerCallContext context)
    {
        if (request.SeatId <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "seat_id skal vaere stoerre end 0"));

        // TODO: tjek saedet i databasen og opret et SeatHold (NotFound / AlreadyExists ved fejl)
        return Task.FromResult(new ReserveSeatResponse
        {
            Success = true,
            Status = "Held",
            ExpiresAt = Timestamp.FromDateTime(DateTime.UtcNow.AddMinutes(5))
        });
    }

    public override async Task SeatSelection(
        IAsyncStreamReader<SeatAction> requestStream,
        IServerStreamWriter<SeatUpdate> responseStream,
        ServerCallContext context)
    {
        // TODO: send opdateringer til ALLE klienter i samme forestilling (broadcaster)
        await foreach (var action in requestStream.ReadAllAsync(context.CancellationToken))
        {
            await responseStream.WriteAsync(new SeatUpdate
            {
                SeatId = action.SeatId,
                Status = action.Action == SeatActionType.Select ? "Held" : "Released",
                ByClient = action.ClientId
            });
        }
    }
}
