using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Commands;

public record CreateMovieCommand(CreateMovieDto Movie, int UserId) : IRequest<MovieDto>;