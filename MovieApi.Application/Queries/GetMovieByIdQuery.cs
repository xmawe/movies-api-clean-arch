using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Queries;

public record GetMovieByIdQuery(int MovieId, int UserId) : IRequest<MovieDto?>;