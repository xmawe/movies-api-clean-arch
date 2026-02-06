using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieApi.Application.DTOs;
using MovieApi.Application.Interfaces;
using MovieApi.Application.Queries;

namespace MovieApi.Application.Handlers;

public class GetMovieStatsQueryHandler : IRequestHandler<GetMovieStatsQuery, MovieStatsDto>
{
    private readonly IApplicationDbContext _context;

    public GetMovieStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MovieStatsDto> Handle(GetMovieStatsQuery request, CancellationToken cancellationToken)
    {
        var movies = await _context.Movies.Where(m => m.UserId == request.UserId).ToListAsync(cancellationToken);

        if (!movies.Any())
        {
            return new MovieStatsDto(
                TotalMovies: 0,
                TotalGenres: 0,
                TopGenre: string.Empty,
                TopGenreCount: 0,
                AverageRating: 0,
                HighestRating: 0,
                LowestRating: 0,
                TotalDirectors: 0,
                MostFeaturedDirector: string.Empty,
                MostFeaturedDirectorCount: 0
            );
        }

        // Total movies
        var totalMovies = movies.Count;

        // Genre statistics
        var genreGroups = movies.GroupBy(m => m.Genre).ToList();
        var totalGenres = genreGroups.Count;
        var topGenreGroup = genreGroups.OrderByDescending(g => g.Count()).First();
        var topGenre = topGenreGroup.Key;
        var topGenreCount = topGenreGroup.Count();

        // Rating statistics
        var averageRating = Math.Round(movies.Average(m => m.Rating), 2);
        var highestRating = movies.Max(m => m.Rating);
        var lowestRating = movies.Min(m => m.Rating);

        // Director statistics
        var directorGroups = movies.GroupBy(m => m.Director).ToList();
        var totalDirectors = directorGroups.Count;
        var topDirectorGroup = directorGroups.OrderByDescending(g => g.Count()).First();
        var mostFeaturedDirector = topDirectorGroup.Key;
        var mostFeaturedDirectorCount = topDirectorGroup.Count();

        return new MovieStatsDto(
            TotalMovies: totalMovies,
            TotalGenres: totalGenres,
            TopGenre: topGenre,
            TopGenreCount: topGenreCount,
            AverageRating: averageRating,
            HighestRating: highestRating,
            LowestRating: lowestRating,
            TotalDirectors: totalDirectors,
            MostFeaturedDirector: mostFeaturedDirector,
            MostFeaturedDirectorCount: mostFeaturedDirectorCount
        );
    }
}