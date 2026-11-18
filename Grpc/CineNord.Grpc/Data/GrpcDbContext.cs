using Microsoft.EntityFrameworkCore;

namespace CineNord.Grpc.Data;

public class GrpcDbContext(DbContextOptions<GrpcDbContext> options) : DbContext(options)
{
    // TODO: tilføj jeres tabeller her, fx
    // public DbSet<Movie> Movies => Set<Movie>();
}
