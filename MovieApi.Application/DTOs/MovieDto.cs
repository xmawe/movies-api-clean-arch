namespace MovieApi.Application.DTOs;

public record MovieDto(
    int Id,
    string Title,
    string Director,
    string Genre,
    int ReleaseYear,
    decimal Rating
);