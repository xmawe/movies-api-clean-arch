using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MovieApi.Application.Commands.Auth;
using MovieApi.Application.DTOs;
using System.Net;
using System.Text.Json;

namespace MovieApi.Functions.HttpTriggers;

public class AuthFunctions
{
    private readonly ILogger<AuthFunctions> _logger;
    private readonly IMediator _mediator;

    public AuthFunctions(ILogger<AuthFunctions> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// URL: POST http://localhost:7071/api/auth/register
    [Function("Register")]
    public async Task<HttpResponseData> Register(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/register")] HttpRequestData req)
    {
        _logger.LogInformation("Register function triggered");

        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonSerializer.Deserialize<RegisterRequest>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (request == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { Error = "Invalid request data" });
                return badResponse;
            }

            var command = new RegisterCommand(request.Username, request.Email, request.Password);
            var result = await _mediator.Send(command);

            _logger.LogInformation($"User registered successfully: {result.Username}");

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed - user already exists");
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(new { Error = ex.Message });
            return badResponse;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Registration failed - validation error");
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(new { Error = ex.Message });
            return badResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { Error = "Failed to register user" });
            return errorResponse;
        }
    }

    /// URL: POST http://localhost:7071/api/auth/login
    [Function("Login")]
    public async Task<HttpResponseData> Login(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/login")] HttpRequestData req)
    {
        _logger.LogInformation("Login function triggered");

        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonSerializer.Deserialize<LoginRequest>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (request == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { Error = "Invalid request data" });
                return badResponse;
            }

            var command = new LoginCommand(request.Email, request.Password);
            var result = await _mediator.Send(command);

            _logger.LogInformation($"User logged in successfully: {result.Username}");

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Login failed - invalid credentials");
            var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await unauthorizedResponse.WriteAsJsonAsync(new { Error = ex.Message });
            return unauthorizedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { Error = "Failed to login" });
            return errorResponse;
        }
    }
}