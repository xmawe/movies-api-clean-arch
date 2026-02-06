namespace MovieApi.Domain.Entities;

public class Movie
{
    public int Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Director { get; private set; } = string.Empty;
    public string Genre { get; private set; } = string.Empty;
    public int ReleaseYear { get; private set; }
    public decimal Rating { get; private set; }
    
    // Add User relationship
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    private Movie() { }

    public static Movie Create(string title, string director, string genre, int releaseYear, decimal rating, int userId)
    {
        ValidateTitle(title);
        ValidateDirector(director);
        ValidateGenre(genre);
        ValidateReleaseYear(releaseYear);
        ValidateRating(rating);

        return new Movie
        {
            Title = title,
            Director = director,
            Genre = genre,
            ReleaseYear = releaseYear,
            Rating = rating,
            UserId = userId
        };
    }

    public void UpdateTitle(string newTitle)
    {
        ValidateTitle(newTitle);
        Title = newTitle;
    }

    public void UpdateDirector(string newDirector)
    {
        ValidateDirector(newDirector);
        Director = newDirector;
    }

    public void UpdateGenre(string newGenre)
    {
        ValidateGenre(newGenre);
        Genre = newGenre;
    }

    public void UpdateReleaseYear(int newReleaseYear)
    {
        ValidateReleaseYear(newReleaseYear);
        ReleaseYear = newReleaseYear;
    }

    public void UpdateRating(decimal newRating)
    {
        ValidateRating(newRating);
        Rating = newRating;
    }

    // Keep all your existing validation methods...
    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (title.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters", nameof(title));
    }

    private static void ValidateDirector(string director)
    {
        if (string.IsNullOrWhiteSpace(director))
            throw new ArgumentException("Director cannot be empty", nameof(director));
        if (director.Length > 100)
            throw new ArgumentException("Director name cannot exceed 100 characters", nameof(director));
    }

    private static void ValidateGenre(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            throw new ArgumentException("Genre cannot be empty", nameof(genre));
        if (genre.Length > 50)
            throw new ArgumentException("Genre cannot exceed 50 characters", nameof(genre));
    }

    private static void ValidateReleaseYear(int releaseYear)
    {
        const int firstMovieYear = 1888;
        int currentYear = DateTime.Now.Year;
        if (releaseYear < firstMovieYear || releaseYear > currentYear + 5)
            throw new ArgumentException(
                $"Release year must be between {firstMovieYear} and {currentYear + 5}", 
                nameof(releaseYear));
    }

    private static void ValidateRating(decimal rating)
    {
        if (rating < 0 || rating > 10)
            throw new ArgumentException("Rating must be between 0 and 10", nameof(rating));
    }
}