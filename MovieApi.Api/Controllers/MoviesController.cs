using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Application.Commands;
using MovieApi.Application.DTOs;
using MovieApi.Application.Queries;
using System.Security.Claims;

namespace MovieApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MoviesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MoviesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
        
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User ID not found in token");
        
        return int.Parse(userIdClaim.Value);
    }

    [HttpGet]
    public async Task<ActionResult<List<MovieDto>>> GetAll()
    {
        var userId = GetUserId();
        var query = new GetAllMoviesQuery(userId);
        var movies = await _mediator.Send(query);
        return Ok(movies);
    }
    
    [HttpGet("stats")]
    public async Task<ActionResult<MovieStatsDto>> GetStats()
    {
        var userId = GetUserId();
        var stats = await _mediator.Send(new GetMovieStatsQuery(userId));
        return Ok(stats);
    }
    
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<MovieDto>>> Search([FromQuery] string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return BadRequest("Keyword cannot be empty");
        }
        var userId = GetUserId();
        var movies = await _mediator.Send(new SearchMoviesQuery(keyword, userId));
        return Ok(movies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MovieDto>> GetById(int id)
    {
        var userId = GetUserId();
        var query = new GetMovieByIdQuery(id, userId);
        var movie = await _mediator.Send(query);
        
        if (movie == null)
            return NotFound(new { message = "Movie not found" });
        
        return Ok(movie);
    }

    [HttpPost]
    public async Task<ActionResult<MovieDto>> Create([FromBody] CreateMovieDto dto)
    {
        try
        {
            var userId = GetUserId();
            var command = new CreateMovieCommand(dto, userId);
            var movie = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MovieDto>> Update(int id, [FromBody] UpdateMovieDto dto)
    {
        try
        {
            var userId = GetUserId();
            var command = new UpdateMovieCommand(id, dto, userId);
            var movie = await _mediator.Send(command);
            return Ok(movie);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var userId = GetUserId();
            var command = new DeleteMovieCommand(id, userId);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }
}