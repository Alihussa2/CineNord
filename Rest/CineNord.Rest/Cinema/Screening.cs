namespace CineNord.Rest.Models;

public class Screening
{
    public int Id { get; set; }

    public int MovieId { get; set; }
    public Movie? Movie { get; set; }

    public int AuditoriumId { get; set; }
    public Auditorium? Auditorium { get; set; }

    public DateTime StartsAt { get; set; }
    public decimal Price { get; set; }
}