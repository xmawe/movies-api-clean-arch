using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Commands.Auth;

public record RegisterCommand(string Username, string Email, string Password) 
    : IRequest<AuthResponse>;