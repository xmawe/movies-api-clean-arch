using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieApi.Application.Interfaces;

namespace MovieApi.Application.Commands;

public class DeleteMovieCommandHandler : IRequestHandler<DeleteMovieCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteMovieCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = await _context.Movies
            .FirstOrDefaultAsync(m => m.Id == request.MovieId, cancellationToken);

        if (movie == null)
            throw new KeyNotFoundException($"Movie with ID {request.MovieId} not found");

        // Verify ownership
        if (movie.UserId != request.UserId)
            throw new UnauthorizedAccessException("You do not have permission to delete this movie");

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}