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
    public DbSet<User> Users { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(20).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(100).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        // Movie configuration
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Title).HasMaxLength(200).IsRequired();
            entity.Property(m => m.Director).HasMaxLength(100).IsRequired();
            entity.Property(m => m.Genre).HasMaxLength(50).IsRequired();
            entity.Property(m => m.Rating).HasPrecision(3, 1);

            // Relationship
            entity.HasOne(m => m.User)
                .WithMany(u => u.Movies)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // // Seed data using the factory method with Id
        // modelBuilder.Entity<Movie>().HasData(
        //     Movie.CreateWithId(1, "The Shawshank Redemption", "Frank Darabont", "Drama", 1994, 9.3m, 1),
        //     Movie.CreateWithId(2, "The Godfather", "Francis Ford Coppola", "Crime", 1972, 9.2m, 1),
        //     Movie.CreateWithId(3, "The Dark Knight", "Christopher Nolan", "Action", 2008, 9.0m, 1)
        // );
    }

    private Movie CreateMovieForSeed(int id, string title, string director, string genre, int releaseYear, decimal rating, int userId)
    {
        var movie = Movie.Create(title, director, genre, releaseYear, rating, userId);
        
        var idProperty = typeof(Movie).GetProperty(nameof(Movie.Id));
        idProperty?.SetValue(movie, id);
        
        return movie;
    }
}