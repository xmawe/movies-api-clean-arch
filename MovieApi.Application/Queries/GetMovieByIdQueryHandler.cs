using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieApi.Application.DTOs;
using MovieApi.Application.Interfaces;

namespace MovieApi.Application.Queries;

public class GetMovieByIdQueryHandler : IRequestHandler<GetMovieByIdQuery, MovieDto?>
{
    private readonly IApplicationDbContext _context;

    public GetMovieByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MovieDto?> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Movies
            .Where(m => m.Id == request.Id)
            .Select(m => new MovieDto(
                m.Id,
                m.Title,
                m.Director,
                m.Genre,
                m.ReleaseYear,
                m.Rating
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}