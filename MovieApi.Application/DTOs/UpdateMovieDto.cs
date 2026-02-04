namespace MovieApi.Application.DTOs;

public record UpdateMovieDto(
    string Title,
    string Director,
    string Genre,
    int ReleaseYear,
    decimal Rating
);
