using Microsoft.EntityFrameworkCore;
using MovieApi.Application.Interfaces;
using MovieApi.Domain.Entities;

namespace MovieApi.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Director).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Genre).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ReleaseYear).IsRequired();
            entity.Property(e => e.Rating).HasPrecision(3, 1).IsRequired();
        });

        // Seed data using the factory method with Id
        modelBuilder.Entity<Movie>().HasData(
            Movie.CreateWithId(1, "The Shawshank Redemption", "Frank Darabont", "Drama", 1994, 9.3m),
            Movie.CreateWithId(2, "The Godfather", "Francis Ford Coppola", "Crime", 1972, 9.2m),
            Movie.CreateWithId(3, "The Dark Knight", "Christopher Nolan", "Action", 2008, 9.0m)
        );
    }

    private Movie CreateMovieForSeed(int id, string title, string director, string genre, int releaseYear, decimal rating)
    {
        var movie = Movie.Create(title, director, genre, releaseYear, rating);
        
        var idProperty = typeof(Movie).GetProperty(nameof(Movie.Id));
        idProperty?.SetValue(movie, id);
        
        return movie;
    }
}