namespace MovieApi.Application.DTOs;

public record UserDto(
    int Id,
    string Username,
    string Email,
    string Password
);