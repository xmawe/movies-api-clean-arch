using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Queries;

public record GetAllMoviesQuery : IRequest<IEnumerable<MovieDto>>;