namespace MovieApi.Application.DTOs;

public record CreateMovieDto(
    string Title,
    string Director,
    string Genre,
    int ReleaseYear,
    decimal Rating
);
