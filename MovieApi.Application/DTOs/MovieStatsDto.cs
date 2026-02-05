namespace MovieApi.Application.DTOs;

public record MovieStatsDto(
    int TotalMovies,
    int TotalGenres,
    string TopGenre,
    int TopGenreCount,
    decimal AverageRating,
    decimal HighestRating,
    decimal LowestRating,
    int TotalDirectors,
    string MostFeaturedDirector,
    int MostFeaturedDirectorCount
);