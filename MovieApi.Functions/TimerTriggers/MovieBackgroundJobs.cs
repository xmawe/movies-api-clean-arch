using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MovieApi.Application.Queries;

namespace MovieApi.Functions.TimerTriggers;

public class MovieBackgroundJobs
{
    private readonly ILogger<MovieBackgroundJobs> _logger;
    private readonly IMediator _mediator;

    public MovieBackgroundJobs(ILogger<MovieBackgroundJobs> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    
    /// <summary>
    /// Check for data inconsistencies
    /// Runs every 6 hours
    /// </summary>
    [Function("DataQualityCheck")]
    public async Task DataQualityCheck([TimerTrigger("0 0 */6 * * *")] TimerInfo timer)
    {
        _logger.LogInformation($"Data quality check started at: {DateTime.UtcNow}");

        try
        {
            var movies = await _mediator.Send(new GetAllMoviesQuery());

            // Perform data quality checks
            var moviesWithoutTitle = movies.Count(m => string.IsNullOrWhiteSpace(m.Title));
            var moviesWithInvalidYear = movies.Count(m => m.ReleaseYear < 1888 || m.ReleaseYear > DateTime.UtcNow.Year + 5);
            
            _logger.LogInformation("Data Quality Report:");
            _logger.LogInformation($"- Total Movies: {movies.Count()}");
            _logger.LogInformation($"- Movies without title: {moviesWithoutTitle}");
            _logger.LogInformation($"- Movies with invalid year: {moviesWithInvalidYear}");

            if (moviesWithoutTitle > 0 || moviesWithInvalidYear > 0)
            {
                _logger.LogWarning($"Found {moviesWithoutTitle + moviesWithInvalidYear} data quality issues!");
                // Send alert, create ticket, etc.
            }
            else
            {
                _logger.LogInformation("✓ All data quality checks passed!");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during data quality check");
        }
    }

    /// <summary>
    /// Sync with external movie database API
    /// Runs every day at 3:00 AM UTC
    /// </summary>
    [Function("SyncExternalMovieData")]
    public async Task SyncExternalMovieData([TimerTrigger("0 0 3 * * *")] TimerInfo timer)
    {
        _logger.LogInformation($"External movie data sync started at: {DateTime.UtcNow}");

        try
        {
            // Get current movies from database
            var existingMovies = await _mediator.Send(new GetAllMoviesQuery());
            _logger.LogInformation($"Current movies in database: {existingMovies.Count()}");

            // A real implementation here

            _logger.LogInformation("External sync completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during external sync");
        }
    }

    /// <summary>
    /// Test timer - fires every 30 seconds for testing
    /// </summary>
    [Function("TestTimer")]
    public async Task TestTimer([TimerTrigger("*/30 * * * * *")] TimerInfo timer)
    {
        _logger.LogInformation($"------- Test timer fired at: {DateTime.UtcNow:HH:mm:ss}");

        try
        {
            // Quick test: get movie count
            var movies = await _mediator.Send(new GetAllMoviesQuery());
            _logger.LogInformation($"   Current movie count: {movies.Count()}");
            _logger.LogInformation($"   Next run: {timer.ScheduleStatus?.Next:HH:mm:ss}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in test timer");
        }
    }
}