using MediatR;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Application.Commands;
using MovieApi.Application.DTOs;
using MovieApi.Application.Queries;

namespace MovieApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MoviesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetAll()
    {
        var movies = await _mediator.Send(new GetAllMoviesQuery());
        return Ok(movies);
    }
    
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<MovieDto>>> Search([FromQuery] string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return BadRequest("Keyword cannot be empty");
        }

        var movies = await _mediator.Send(new SearchMoviesQuery(keyword));
        return Ok(movies);
    }
    
    [HttpGet("stats")]
    public async Task<ActionResult<MovieStatsDto>> GetStats()
    {
        var stats = await _mediator.Send(new GetMovieStatsQuery());
        return Ok(stats);
    }

    [HttpPost]
    public async Task<ActionResult<MovieDto>> Create([FromBody] MovieDto movieDto)
    {
        var command = new CreateMovieCommand(movieDto);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMovieDto movieDto)
    {
        var command = new UpdateMovieCommand(id, movieDto);
        await _mediator.Send(command);

        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteMovieCommand(id);
        await _mediator.Send(command);

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MovieDto>> GetById(int id)
    {
        var movie = await _mediator.Send(new GetMovieByIdQuery(id));
        return Ok(movie);
    }


}