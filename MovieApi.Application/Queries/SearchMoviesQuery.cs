using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Queries;

public record SearchMoviesQuery(string Keyword) : IRequest<IEnumerable<MovieDto>>;