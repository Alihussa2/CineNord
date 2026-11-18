using Microsoft.EntityFrameworkCore;

namespace CineNord.Sse.Data;

public class SseDbContext(DbContextOptions<SseDbContext> options) : DbContext(options)
{
    // TODO: tilføj jeres tabeller her, fx
    // public DbSet<Movie> Movies => Set<Movie>();
}
