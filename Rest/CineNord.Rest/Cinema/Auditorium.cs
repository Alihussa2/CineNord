namespace CineNord.Rest.Models;

public class Auditorium
{
    public int Id { get; set; }
    public int CinemaId { get; set; }
    public Cinema? Cinema { get; set; }

    public string Name { get; set; } = "";
    public int Rows { get; set; }
    public int SeatsPerRow { get; set; }

    public List<Screening> Screenings { get; set; } = new();
}