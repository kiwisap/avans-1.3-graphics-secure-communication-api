using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GraphicsSecureCommunicationApi.WebApi.Models;
using GraphicsSecureCommunicationApi.WebApi.Repositories;
using GraphicsSecureCommunicationApi.WebApi.Services;

namespace GraphicsSecureCommunicationApi.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnvironmentsController : ControllerBase
{
    private readonly IEnvironment2DRepository _repository;
    private readonly IAuthenticationService _authService;

    public EnvironmentsController(IEnvironment2DRepository repository, IAuthenticationService authService)
    {
        _repository = repository;
        _authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Environment2D>>> GetAll()
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var environments = await _repository.GetAllByUserIdAsync(userId);
        return Ok(environments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Environment2D>> GetById(int id)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var environment = await _repository.GetByIdAsync(id, userId);
        if (environment == null)
            return NotFound();

        return Ok(environment);
    }

    [HttpPost]
    public async Task<ActionResult<Environment2D>> Create(Environment2D environment)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        environment.UserId = userId;
        var created = await _repository.CreateAsync(environment);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Environment2D environment)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (id != environment.Id)
            return BadRequest();

        var success = await _repository.UpdateAsync(environment, userId);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var success = await _repository.DeleteAsync(id, userId);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
