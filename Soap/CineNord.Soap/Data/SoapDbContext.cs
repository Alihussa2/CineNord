using Microsoft.EntityFrameworkCore;

namespace CineNord.Soap.Data;

public class SoapDbContext(DbContextOptions<SoapDbContext> options) : DbContext(options)
{
    // TODO: tilføj jeres tabeller her, fx
    // public DbSet<Movie> Movies => Set<Movie>();
}
