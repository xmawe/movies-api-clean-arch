using MediatR;
using MovieApi.Application.DTOs;
using MovieApi.Application.Interfaces;
using MovieApi.Domain.Entities;

namespace MovieApi.Application.Commands;

public class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommand, MovieDto>
{
    private readonly IApplicationDbContext _context;

    public CreateMovieCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MovieDto> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = new Movie
        {
            Title = request.Movie.Title,
            Director = request.Movie.Director,
            Genre = request.Movie.Genre,
            ReleaseYear = request.Movie.ReleaseYear,
            Rating = request.Movie.Rating
        };

        _context.Movies.Add(movie);
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