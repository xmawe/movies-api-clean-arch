using MediatR;
using MovieApi.Application.DTOs;

namespace MovieApi.Application.Queries;

public record GetMovieByIdQuery(int Id) : IRequest<MovieDto?>;