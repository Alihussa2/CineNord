using System.ComponentModel.DataAnnotations;

namespace CineNord.Rest.Dtos;

// Det klienten sender ved POST og PUT. Valideres automatisk af [ApiController].
public class MovieRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    [RegularExpression(@"^[^<>]*$", ErrorMessage = "Titlen må ikke indeholde < eller >")]
    public string Title { get; set; } = "";

    [Required]
    [RegularExpression("^(action|animation|comedy|drama|musical|sci-fi)$",
        ErrorMessage = "Genre skal være action, animation, comedy, drama, musical eller sci-fi")]
    public string Genre { get; set; } = "";

    [Range(1, 600)]
    public int DurationMinutes { get; set; }

    [Required]
    [RegularExpression("^(A|7|11|15)$", ErrorMessage = "AgeRating skal være A, 7, 11 eller 15")]
    public string AgeRating { get; set; } = "";

    [Range(1888, 2100)]
    public int ReleaseYear { get; set; }
}