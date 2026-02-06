namespace MovieApi.Application.DTOs;

public class CreateMovieDto
{
    public string Title { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public decimal Rating { get; set; }
}