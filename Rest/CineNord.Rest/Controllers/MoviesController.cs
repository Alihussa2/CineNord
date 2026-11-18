using CineNord.Rest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using CineNord.Rest.Dtos;
using CineNord.Rest.Models;

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

    // GET /v1/movies/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMovie(int id)
    {
        var movie = await db.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

        if (movie is null)
            return NotFound(new { error = $"Film med id {id} findes ikke" });

        return Ok(new
        {
            movie.Id,
            movie.Title,
            movie.Genre,
            movie.DurationMinutes,
            movie.AgeRating,
            movie.ReleaseYear,
            _links = MovieLinks(movie.Id)
        });
    }

    // POST /v1/movies
    [HttpPost]
    public async Task<IActionResult> CreateMovie(MovieRequest request)
    {
        var movie = new Movie();
        Apply(movie, request);

        db.Movies.Add(movie);
        await db.SaveChangesAsync();

        // 201 Created + Location-header der peger på den nye film
        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, ToDto(movie));
    }

    // PUT /v1/movies/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMovie(int id, MovieRequest request)
    {
        var movie = await db.Movies.FindAsync(id);
        if (movie is null)
            return NotFound(new { error = $"Film med id {id} findes ikke" });

        Apply(movie, request);
        await db.SaveChangesAsync();

        return Ok(ToDto(movie));
    }

    // DELETE /v1/movies/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var movie = await db.Movies.FindAsync(id);
        if (movie is null)
            return NotFound(new { error = $"Film med id {id} findes ikke" });

        if (await db.Screenings.AnyAsync(sc => sc.MovieId == id))
            return Conflict(new { error = "Filmen har forestillinger og kan ikke slettes" });

        db.Movies.Remove(movie);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // Kopierer de tilladte felter fra request til entitet
    private static void Apply(Movie movie, MovieRequest r)
    {
        movie.Title = r.Title.Trim();
        movie.Genre = r.Genre;
        movie.DurationMinutes = r.DurationMinutes;
        movie.AgeRating = r.AgeRating;
        movie.ReleaseYear = r.ReleaseYear;
    }

    // Samme svarformat for én film overalt
    private object ToDto(Movie m) => new
    {
        m.Id,
        m.Title,
        m.Genre,
        m.DurationMinutes,
        m.AgeRating,
        m.ReleaseYear,
        _links = MovieLinks(m.Id)
    };

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