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
        var movie = Movie.Create(
            request.Movie.Title,
            request.Movie.Director,
            request.Movie.Genre,
            request.Movie.ReleaseYear,
            request.Movie.Rating
        );

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