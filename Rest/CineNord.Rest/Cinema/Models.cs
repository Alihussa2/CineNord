namespace CineNord.Rest.Models;

public class Cinema
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string City { get; set; } = "";
    public string Address { get; set; } = "";

    public List<Auditorium> Auditoriums { get; set; } = new();
}