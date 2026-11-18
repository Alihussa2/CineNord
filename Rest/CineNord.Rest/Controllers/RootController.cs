using Microsoft.AspNetCore.Mvc;

namespace CineNord.Rest.Controllers;

// Indgangen til API'et: top-level HATEOAS-links
[ApiController]
[Route("v1")]
public class RootController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        _links = new
        {
            self = new { href = "/v1" },
            movies = new { href = "/v1/movies" },
            cinemas = new { href = "/v1/cinemas" },
            screenings = new { href = "/v1/screenings" },
            login = new { href = "/v1/auth/login", method = "POST" }
        }
    });
}
