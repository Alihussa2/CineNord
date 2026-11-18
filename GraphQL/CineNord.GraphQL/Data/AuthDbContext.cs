using Microsoft.EntityFrameworkCore;

namespace CineNord.GraphQL.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    // TODO: tilføj jeres tabeller her, fx
    // public DbSet<Movie> Movies => Set<Movie>();
}
