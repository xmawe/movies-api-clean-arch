using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Queries;

public record GetAllMoviesQuery(int UserId) : IRequest<List<MovieDto>>;