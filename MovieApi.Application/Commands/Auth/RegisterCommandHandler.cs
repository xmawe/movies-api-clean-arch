using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieApi.Application.DTOs;
using MovieApi.Application.Interfaces;
using MovieApi.Application.Services;
using MovieApi.Domain.Entities;

namespace MovieApi.Application.Commands.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCommandHandler(IApplicationDbContext context, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists");

        var existingUsername = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);
        
        if (existingUsername != null)
            throw new InvalidOperationException("Username is already taken");

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user
        var user = User.Create(request.Username, request.Email, passwordHash);
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate token
        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponse(token, user.Username, user.Email);
    }
}