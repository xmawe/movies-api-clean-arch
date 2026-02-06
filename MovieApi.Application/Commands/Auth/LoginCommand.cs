using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Commands.Auth;

public record LoginCommand(string Email, string Password) 
    : IRequest<AuthResponse>;