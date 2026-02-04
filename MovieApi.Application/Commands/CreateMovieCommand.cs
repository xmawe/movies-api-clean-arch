using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Commands;

public record CreateMovieCommand(MovieDto Movie) : IRequest<MovieDto>;