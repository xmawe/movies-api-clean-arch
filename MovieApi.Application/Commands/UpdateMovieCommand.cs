using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Commands;

public record UpdateMovieCommand(int MovieId, UpdateMovieDto Movie, int UserId) : IRequest<MovieDto>;