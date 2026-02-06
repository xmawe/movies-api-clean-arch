using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieApi.Application.DTOs;
using MovieApi.Application.Interfaces;

namespace MovieApi.Application.Commands;

public class UpdateMovieCommandHandler : IRequestHandler<UpdateMovieCommand, MovieDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateMovieCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MovieDto> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = await _context.Movies
            .FirstOrDefaultAsync(m => m.Id == request.MovieId, cancellationToken);

        if (movie == null)
            throw new KeyNotFoundException($"Movie with ID {request.MovieId} not found");

        // Verify ownership
        if (movie.UserId != request.UserId)
            throw new UnauthorizedAccessException("You do not have permission to update this movie");

        movie.UpdateTitle(request.Movie.Title);
        movie.UpdateDirector(request.Movie.Director);
        movie.UpdateGenre(request.Movie.Genre);
        movie.UpdateReleaseYear(request.Movie.ReleaseYear);
        movie.UpdateRating(request.Movie.Rating);

        await _context.SaveChangesAsync(cancellationToken);

        return new MovieDto
        (
            movie.Id,
            movie.Title,
            movie.Director,
            movie.Genre,
            movie.ReleaseYear,
            movie.Rating
        );
    }
}