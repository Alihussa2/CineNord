using CineNord.Soap.Contracts;

namespace CineNord.Soap.Services;

public class BookingService : IBookingService
{
    public string Ping(string message) => $"CineNord SOAP svarer: {message}";
}
