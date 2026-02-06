using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieApi.Application.DTOs;
using MovieApi.Application.Interfaces;
using MovieApi.Application.Queries;

namespace MovieApi.Application.Handlers;

public class SearchMoviesQueryHandler : IRequestHandler<SearchMoviesQuery, IEnumerable<MovieDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchMoviesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MovieDto>> Handle(SearchMoviesQuery request, CancellationToken cancellationToken)
    {
        var searchTerm = request.Keyword.ToLower();

        var movies = await _context.Movies
            .Where(m => 
                m.UserId == request.UserId)
            .Where(m => 
                m.Title.ToLower().Contains(searchTerm) ||
                m.Director.ToLower().Contains(searchTerm) ||
                m.Genre.ToLower().Contains(searchTerm) ||
                m.ReleaseYear.ToString().Contains(searchTerm) ||
                m.Rating.ToString().Contains(searchTerm)
            )
            .ToListAsync(cancellationToken);

        return movies.Select(m => new MovieDto(
            m.Id,
            m.Title,
            m.Director,
            m.Genre,
            m.ReleaseYear,
            m.Rating
        ));
    }
}