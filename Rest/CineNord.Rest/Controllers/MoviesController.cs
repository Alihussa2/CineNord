using CineNord.Rest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace CineNord.Rest.Controllers;

[ApiController]
[Route("v1/movies")]
public class MoviesController(RestDbContext db) : ControllerBase
{
    // GET /v1/movies?s=dune&genre=sci-fi&page=1&size=10
    [HttpGet]
    public async Task<IActionResult> GetMovies(
        [FromQuery] string? s,
        [FromQuery] string? genre,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        if (page < 1 || size < 1 || size > 100)
            return BadRequest(new { error = "page skal være mindst 1, og size skal være mellem 1 og 100" });

        // 1. Byg forespørgslen (EF Core laver parametre, så ingen SQL-injection)
        var query = db.Movies.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(s))
            query = query.Where(m => EF.Functions.ILike(m.Title, $"%{s}%"));

        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(m => m.Genre == genre.ToLower());

        // 2. Tæl og hent den ønskede side
        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)size);

        var movies = await query
            .OrderBy(m => m.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        // 3. Links til navigation mellem sider (beholder s og genre)
        string PageUrl(int p) => QueryHelpers.AddQueryString("/v1/movies",
            new Dictionary<string, string?>
            {
                ["s"] = s,
                ["genre"] = genre,
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

        // 4. Svaret i samme format som i REST-slides
        return Ok(new
        {
            _links = links,
            page = new { number = page, size, totalItems, totalPages },
            _embedded = new
            {
                movies = movies.Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.Genre,
                    m.DurationMinutes,
                    m.AgeRating,
                    m.ReleaseYear,
                    _links = MovieLinks(m.Id)
                })
            }
        });
    }

    // Kontekstuelle links til én film
    private Dictionary<string, object> MovieLinks(int id)
    {
        var links = new Dictionary<string, object>
        {
            ["self"] = new { href = $"/v1/movies/{id}" },
            ["screenings"] = new { href = $"/v1/screenings?movie_id={id}" }
        };

        // update/delete vises kun for brugere, der er logget ind (virker når JWT er på plads)
        if (User.Identity?.IsAuthenticated == true)
        {
            links["update"] = new { href = $"/v1/movies/{id}", method = "PUT" };
            links["delete"] = new { href = $"/v1/movies/{id}", method = "DELETE" };
        }

        return links;
    }
}