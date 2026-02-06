using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieApi.Application.DTOs;
using MovieApi.Application.Interfaces;

namespace MovieApi.Application.Queries;

public class GetAllMoviesQueryHandler : IRequestHandler<GetAllMoviesQuery, List<MovieDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllMoviesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MovieDto>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Movies
            .Where(m => m.UserId == request.UserId)  // Filter by user
            .Select(m => new MovieDto
            (
                m.Id,
                m.Title,
                m.Director,
                m.Genre,
                m.ReleaseYear,
                m.Rating
            ))
            .ToListAsync(cancellationToken);
    }
}