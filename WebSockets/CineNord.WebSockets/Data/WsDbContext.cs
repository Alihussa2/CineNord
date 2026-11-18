using Microsoft.EntityFrameworkCore;

namespace CineNord.WebSockets.Data;

public class WsDbContext(DbContextOptions<WsDbContext> options) : DbContext(options)
{
    // TODO: tilføj jeres tabeller her, fx
    // public DbSet<Movie> Movies => Set<Movie>();
}
