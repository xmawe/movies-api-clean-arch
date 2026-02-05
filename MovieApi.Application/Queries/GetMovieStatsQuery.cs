using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Queries;

public record GetMovieStatsQuery : IRequest<MovieStatsDto>;