using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MovieApi.Application.Commands;
using MovieApi.Application.DTOs;
using MovieApi.Application.Queries;
using MovieApi.Application.Services;
using System.Net;
using System.Text.Json;
using MovieApi.Application.Interfaces;

namespace MovieApi.Functions.HttpTriggers;

public class MovieFunctions
{
    private readonly ILogger<MovieFunctions> _logger;
    private readonly IMediator _mediator;
    private readonly IJwtTokenService _jwtTokenService;

    public MovieFunctions(ILogger<MovieFunctions> logger, IMediator mediator, IJwtTokenService jwtTokenService)
    {
        _logger = logger;
        _mediator = mediator;
        _jwtTokenService = jwtTokenService;
    }

    private int? GetUserIdFromToken(HttpRequestData req)
    {
        if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
            return null;

        var authHeader = authHeaders.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return null;

        var token = authHeader.Substring("Bearer ".Length).Trim();
        return _jwtTokenService.ValidateToken(token);
    }

    private async Task<HttpResponseData> UnauthorizedResponse(HttpRequestData req, string message = "Unauthorized")
    {
        var response = req.CreateResponse(HttpStatusCode.Unauthorized);
        await response.WriteAsJsonAsync(new { Error = message });
        return response;
    }

    /// URL: GET http://localhost:7071/api/movies
    [Function("GetAllMovies")]
    public async Task<HttpResponseData> GetAllMovies(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "movies")] HttpRequestData req)
    {
        _logger.LogInformation("GetAllMovies function triggered");

        try
        {
            var userId = GetUserIdFromToken(req);
            if (userId == null)
                return await UnauthorizedResponse(req);

            var movies = await _mediator.Send(new GetAllMoviesQuery(userId.Value));

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(movies);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all movies");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { Error = "Failed to retrieve movies" });
            return errorResponse;
        }
    }

    /// URL: GET http://localhost:7071/api/movies/1
    [Function("GetMovieById")]
    public async Task<HttpResponseData> GetMovieById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "movies/{id:int}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation($"GetMovieById function triggered for ID: {id}");

        try
        {
            var userId = GetUserIdFromToken(req);
            if (userId == null)
                return await UnauthorizedResponse(req);

            var movie = await _mediator.Send(new GetMovieByIdQuery(id, userId.Value));

            if (movie == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteAsJsonAsync(new { Error = $"Movie with ID {id} not found" });
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(movie);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting movie {id}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { Error = "Failed to retrieve movie" });
            return errorResponse;
        }
    }

    /// URL: POST http://localhost:7071/api/movies
    [Function("CreateMovie")]
    public async Task<HttpResponseData> CreateMovie(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "movies")] HttpRequestData req)
    {
        _logger.LogInformation("CreateMovie function triggered");

        try
        {
            var userId = GetUserIdFromToken(req);
            if (userId == null)
                return await UnauthorizedResponse(req);

            // Read and deserialize request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var movieDto = JsonSerializer.Deserialize<CreateMovieDto>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (movieDto == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { Error = "Invalid movie data" });
                return badResponse;
            }

            var command = new CreateMovieCommand(movieDto, userId.Value);
            var result = await _mediator.Send(command);

            _logger.LogInformation($"Movie created with ID: {result.Id}");

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating movie");
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(new { Error = ex.Message });
            return badResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating movie");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { Error = "Failed to create movie" });
            return errorResponse;
        }
    }

    /// URL: PUT http://localhost:7071/api/movies/1
    [Function("UpdateMovie")]
    public async Task<HttpResponseData> UpdateMovie(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "movies/{id:int}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation($"UpdateMovie function triggered for ID: {id}");

        try
        {
            var userId = GetUserIdFromToken(req);
            if (userId == null)
                return await UnauthorizedResponse(req);

            // Read and deserialize request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var movieDto = JsonSerializer.Deserialize<UpdateMovieDto>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (movieDto == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { Error = "Invalid movie data" });
                return badResponse;
            }

            var command = new UpdateMovieCommand(id, movieDto, userId.Value);
            await _mediator.Send(command);

            _logger.LogInformation($"Movie {id} updated successfully");

            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, $"Movie {id} not found");
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteAsJsonAsync(new { Error = ex.Message });
            return notFoundResponse;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, $"Unauthorized access to movie {id}");
            var forbiddenResponse = req.CreateResponse(HttpStatusCode.Forbidden);
            await forbiddenResponse.WriteAsJsonAsync(new { Error = "You do not have permission to update this movie" });
            return forbiddenResponse;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error updating movie");
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(new { Error = ex.Message });
            return badResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating movie {id}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { Error = "Failed to update movie" });
            return errorResponse;
        }
    }

    /// URL: DELETE http://localhost:7071/api/movies/1
    [Function("DeleteMovie")]
    public async Task<HttpResponseData> DeleteMovie(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "movies/{id:int}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation($"DeleteMovie function triggered for ID: {id}");

        try
        {
            var userId = GetUserIdFromToken(req);
            if (userId == null)
                return await UnauthorizedResponse(req);

            var command = new DeleteMovieCommand(id, userId.Value);
            await _mediator.Send(command);

            _logger.LogInformation($"Movie {id} deleted successfully");

            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, $"Movie {id} not found");
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteAsJsonAsync(new { Error = ex.Message });
            return notFoundResponse;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, $"Unauthorized access to movie {id}");
            var forbiddenResponse = req.CreateResponse(HttpStatusCode.Forbidden);
            await forbiddenResponse.WriteAsJsonAsync(new { Error = "You do not have permission to delete this movie" });
            return forbiddenResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting movie {id}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { Error = "Failed to delete movie" });
            return errorResponse;
        }
    }
}