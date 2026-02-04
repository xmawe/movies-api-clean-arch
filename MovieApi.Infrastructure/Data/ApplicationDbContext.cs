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

        // Seed data
        modelBuilder.Entity<Movie>().HasData(
            new Movie { Id = 1, Title = "The Shawshank Redemption", Director = "Frank Darabont", Genre = "Drama", ReleaseYear = 1994, Rating = 9.3m },
            new Movie { Id = 2, Title = "The Godfather", Director = "Francis Ford Coppola", Genre = "Crime", ReleaseYear = 1972, Rating = 9.2m },
            new Movie { Id = 3, Title = "The Dark Knight", Director = "Christopher Nolan", Genre = "Action", ReleaseYear = 2008, Rating = 9.0m }
        );
    }
}