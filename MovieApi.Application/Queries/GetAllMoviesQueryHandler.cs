using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieApi.Application.DTOs;
using MovieApi.Application.Interfaces;

namespace MovieApi.Application.Queries;

public class GetAllMoviesQueryHandler : IRequestHandler<GetAllMoviesQuery, IEnumerable<MovieDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllMoviesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MovieDto>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
    {
        var movies = await _context.Movies
            .Select(m => new MovieDto(
                m.Id,
                m.Title,
                m.Director,
                m.Genre,
                m.ReleaseYear,
                m.Rating
            ))
            .ToListAsync(cancellationToken);

        return movies;
    }
}