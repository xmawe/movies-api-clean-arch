using MediatR;
using MovieApi.Application.DTOs;
using MovieApi.Application.Interfaces;

namespace MovieApi.Application.Commands;

public class UpdateMovieCommandHandler : IRequestHandler<UpdateMovieCommand, MovieDto?>
{
    private readonly IApplicationDbContext _context;

    public UpdateMovieCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MovieDto?> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = await _context.Movies.FindAsync(
            new object[] { request.Id },
            cancellationToken
        );

        if (movie == null)
            return null;

        movie.Title = request.Movie.Title;
        movie.Director = request.Movie.Director;
        movie.Genre = request.Movie.Genre;
        movie.ReleaseYear = request.Movie.ReleaseYear;
        movie.Rating = request.Movie.Rating;

        await _context.SaveChangesAsync(cancellationToken);

        return new MovieDto(
            movie.Id,
            movie.Title,
            movie.Director,
            movie.Genre,
            movie.ReleaseYear,
            movie.Rating
        );
    }
}