using CineNord.Rest.Models;
using Microsoft.EntityFrameworkCore;

namespace CineNord.Rest.Data;

public class RestDbContext(DbContextOptions<RestDbContext> options) : DbContext(options)
{
    public DbSet<Cinema> Cinemas => Set<Cinema>();
    public DbSet<Auditorium> Auditoriums => Set<Auditorium>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Screening> Screenings => Set<Screening>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Screening>()
            .Property(s => s.Price)
            .HasPrecision(8, 2);

        modelBuilder.Entity<Movie>()
            .HasIndex(m => m.Title);

        SeedData.Apply(modelBuilder);
    }
}