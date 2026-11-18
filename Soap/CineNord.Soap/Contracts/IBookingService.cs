using System.ServiceModel;

namespace CineNord.Soap.Contracts;

[ServiceContract(Namespace = "http://cinenord.dk/booking")]
public interface IBookingService
{
    // Bare til at teste opsaetningen. Erstat med GetBooking, GetAvailableSeats,
    // CreateBooking og CancelBooking (se planen).
    [OperationContract]
    string Ping(string message);
}
