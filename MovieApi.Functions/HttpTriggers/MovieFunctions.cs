using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MovieApi.Application.Commands;
using MovieApi.Application.DTOs;
using MovieApi.Application.Queries;
using System.Net;
using System.Text.Json;

namespace MovieApi.Functions.HttpTriggers;

public class MovieFunctions
{
    private readonly ILogger<MovieFunctions> _logger;
    private readonly IMediator _mediator;

    public MovieFunctions(ILogger<MovieFunctions> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    /// URL: GET http://localhost:7071/api/movies
    [Function("GetAllMovies")]
    public async Task<HttpResponseData> GetAllMovies(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "movies")] HttpRequestData req)
    {
        _logger.LogInformation("GetAllMovies function triggered");

        try
        {
            var movies = await _mediator.Send(new GetAllMoviesQuery());

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
            var movie = await _mediator.Send(new GetMovieByIdQuery(id));

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
            // Read and deserialize request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var movieDto = JsonSerializer.Deserialize<MovieDto>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (movieDto == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { Error = "Invalid movie data" });
                return badResponse;
            }
            
            var command = new CreateMovieCommand(movieDto);
            var result = await _mediator.Send(command);

            _logger.LogInformation($"Movie created with ID: {result.Id}");

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(result);
            return response;
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
            
            var command = new UpdateMovieCommand(id, movieDto);
            await _mediator.Send(command);

            _logger.LogInformation($"Movie {id} updated successfully");

            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
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
            var command = new DeleteMovieCommand(id);
            await _mediator.Send(command);

            _logger.LogInformation($"Movie {id} deleted successfully");

            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
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