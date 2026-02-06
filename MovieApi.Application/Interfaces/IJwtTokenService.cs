using MovieApi.Domain.Entities;

namespace MovieApi.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    int? ValidateToken(string token);
}