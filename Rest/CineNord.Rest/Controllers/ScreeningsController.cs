using CineNord.Rest.Data;
using CineNord.Rest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace CineNord.Rest.Controllers;

[ApiController]
[Route("v1/screenings")]
public class ScreeningsController(RestDbContext db) : ControllerBase
{
    // GET /v1/screenings?s=dune&movie_id=1&cinema_id=1&date=2026-12-01&page=1&size=10
    [HttpGet]
    public async Task<IActionResult> GetScreenings(
        [FromQuery] string? s,
        [FromQuery(Name = "movie_id")] int? movieId,
        [FromQuery(Name = "cinema_id")] int? cinemaId,
        [FromQuery] DateOnly? date,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        if (page < 1 || size < 1 || size > 100)
            return BadRequest(new { error = "page skal være mindst 1, og size skal være mellem 1 og 100" });

        var query = db.Screenings
            .AsNoTracking()
            .Include(sc => sc.Movie)
            .Include(sc => sc.Auditorium)!.ThenInclude(a => a!.Cinema)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(s))
            query = query.Where(sc => EF.Functions.ILike(sc.Movie!.Title, $"%{s}%"));

        if (movieId is not null)
            query = query.Where(sc => sc.MovieId == movieId);

        if (cinemaId is not null)
            query = query.Where(sc => sc.Auditorium!.CinemaId == cinemaId);

        if (date is not null)
        {
            var from = date.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var to = from.AddDays(1);
            query = query.Where(sc => sc.StartsAt >= from && sc.StartsAt < to);
        }

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)size);

        var screenings = await query
            .OrderBy(sc => sc.StartsAt)
            .ThenBy(sc => sc.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        string PageUrl(int p) => QueryHelpers.AddQueryString("/v1/screenings",
            new Dictionary<string, string?>
            {
                ["s"] = s,
                ["movie_id"] = movieId?.ToString(),
                ["cinema_id"] = cinemaId?.ToString(),
                ["date"] = date?.ToString("yyyy-MM-dd"),
                ["page"] = p.ToString(),
                ["size"] = size.ToString()
            }.Where(kv => !string.IsNullOrWhiteSpace(kv.Value)));

        var links = new Dictionary<string, object>
        {
            ["self"] = new { href = PageUrl(page) },
            ["first"] = new { href = PageUrl(1) },
            ["last"] = new { href = PageUrl(Math.Max(totalPages, 1)) }
        };
        if (page > 1) links["prev"] = new { href = PageUrl(page - 1) };
        if (page < totalPages) links["next"] = new { href = PageUrl(page + 1) };

        return Ok(new
        {
            _links = links,
            page = new { number = page, size, totalItems, totalPages },
            _embedded = new { screenings = screenings.Select(ToDto) }
        });
    }

    // GET /v1/screenings/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetScreening(int id)
    {
        var screening = await db.Screenings
            .AsNoTracking()
            .Include(sc => sc.Movie)
            .Include(sc => sc.Auditorium)!.ThenInclude(a => a!.Cinema)
            .FirstOrDefaultAsync(sc => sc.Id == id);

        if (screening is null)
            return NotFound(new { error = $"Forestilling med id {id} findes ikke" });

        return Ok(ToDto(screening));
    }

    // Samme format i liste og enkelt-visning
    private static object ToDto(Screening sc) => new
    {
        sc.Id,
        sc.StartsAt,
        sc.Price,
        movie = new { sc.Movie!.Id, sc.Movie.Title },
        cinema = new { sc.Auditorium!.Cinema!.Id, sc.Auditorium.Cinema.Name, sc.Auditorium.Cinema.City },
        auditorium = new { sc.Auditorium.Id, sc.Auditorium.Name },
        _links = new Dictionary<string, object>
        {
            ["self"] = new { href = $"/v1/screenings/{sc.Id}" },
            ["movie"] = new { href = $"/v1/movies/{sc.MovieId}" },
            ["cinema"] = new { href = $"/v1/cinemas/{sc.Auditorium.CinemaId}" }
        }
    };
}