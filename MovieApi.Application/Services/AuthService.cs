using MovieApi.Application.DTOs;
using MovieApi.Application.Interfaces;
using MovieApi.Domain.Entities;

namespace MovieApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(IApplicationDbContext context, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists");

        var existingUsername = _context.Users.FirstOrDefault(u => u.Username == request.Username);
        if (existingUsername != null)
            throw new InvalidOperationException("Username is already taken");

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user
        var user = User.Create(request.Username, request.Email, passwordHash);
        _context.Users.Add(user);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Generate token
        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponse(token, user.Username, user.Email);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        // Generate token
        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponse(token, user.Username, user.Email);
    }
}