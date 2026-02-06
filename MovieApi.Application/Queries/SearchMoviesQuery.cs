using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Queries;

public record SearchMoviesQuery(string Keyword, int UserId) : IRequest<IEnumerable<MovieDto>>;