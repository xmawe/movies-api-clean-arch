using MediatR;
using MovieApi.Application.Interfaces;

namespace MovieApi.Application.Commands;

public class DeleteMovieCommandHandler : IRequestHandler<DeleteMovieCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteMovieCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = await _context.Movies.FindAsync(
            new object[] { request.Id },
            cancellationToken
        );

        if (movie == null)
            return false;

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}