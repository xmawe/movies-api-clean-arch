using MediatR;

namespace MovieApi.Application.Commands;

public record DeleteMovieCommand(int Id) : IRequest<bool>;