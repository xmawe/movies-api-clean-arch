using MediatR;

namespace MovieApi.Application.Commands;

public record DeleteMovieCommand(int MovieId, int UserId) : IRequest<Unit>;