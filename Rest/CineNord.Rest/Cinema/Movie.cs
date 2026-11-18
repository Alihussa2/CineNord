namespace CineNord.Rest.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Genre { get; set; } = "";
    public int DurationMinutes { get; set; }
    public string AgeRating { get; set; } = "";
    public int ReleaseYear { get; set; }

    public List<Screening> Screenings { get; set; } = new();
}