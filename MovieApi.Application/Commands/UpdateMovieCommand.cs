using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Commands;

public record UpdateMovieCommand(int Id, UpdateMovieDto Movie) : IRequest<MovieDto?>;