using Microsoft.EntityFrameworkCore;

namespace CineNord.Rest.Data;

public class RestDbContext(DbContextOptions<RestDbContext> options) : DbContext(options)
{
    // TODO: tilføj jeres tabeller her, fx
    // public DbSet<Movie> Movies => Set<Movie>();
}
