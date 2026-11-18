using CineNord.Rest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;


namespace CineNord.Rest.Controllers;

[ApiController]
[Route("v1/cinemas")]
public class CinemasController(RestDbContext db) : ControllerBase
{
    // GET /v1/cinemas?s=palads&city=københavn&page=1&size=10
    [HttpGet]
    public async Task<IActionResult> GetCinemas(
        [FromQuery] string? s,
        [FromQuery] string? city,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        if (page < 1 || size < 1 || size > 100)
            return BadRequest(new { error = "page skal være mindst 1, og size skal være mellem 1 og 100" });

        var query = db.Cinemas.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(s))
            query = query.Where(c => EF.Functions.ILike(c.Name, $"%{s}%"));

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(c => EF.Functions.ILike(c.City, city));

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)size);

        var cinemas = await query
            .OrderBy(c => c.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        string PageUrl(int p) => QueryHelpers.AddQueryString("/v1/cinemas",
            new Dictionary<string, string?>
            {
                ["s"] = s,
                ["city"] = city,
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
            _embedded = new
            {
                cinemas = cinemas.Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.City,
                    c.Address,
                    _links = CinemaLinks(c.Id)
                })
            }
        });
    }

    // GET /v1/cinemas/1  (inkl. biografens sale)
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCinema(int id)
    {
        var cinema = await db.Cinemas
            .AsNoTracking()
            .Include(c => c.Auditoriums)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cinema is null)
            return NotFound(new { error = $"Biograf med id {id} findes ikke" });

        return Ok(new
        {
            cinema.Id,
            cinema.Name,
            cinema.City,
            cinema.Address,
            auditoriums = cinema.Auditoriums.Select(a => new
            {
                a.Id,
                a.Name,
                a.Rows,
                a.SeatsPerRow,
                capacity = a.Rows * a.SeatsPerRow
            }),
            _links = CinemaLinks(cinema.Id)
        });
    }

    private static Dictionary<string, object> CinemaLinks(int id) => new()
    {
        ["self"] = new { href = $"/v1/cinemas/{id}" },
        ["screenings"] = new { href = $"/v1/screenings?cinema_id={id}" }
    };
}
