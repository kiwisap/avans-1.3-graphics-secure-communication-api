using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GraphicsSecureCommunicationApi.WebApi.Models.Dto;
using GraphicsSecureCommunicationApi.WebApi.Repositories.Interfaces;
using GraphicsSecureCommunicationApi.WebApi.Services.Interfaces;

namespace GraphicsSecureCommunicationApi.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ObjectsController : ControllerBase
{
    private readonly IObject2DRepository _repository;
    private readonly IAuthenticationService _authService;

    public ObjectsController(IObject2DRepository repository, IAuthenticationService authService)
    {
        _repository = repository;
        _authService = authService;
    }

    [HttpGet("environment/{environmentId}")]
    public async Task<ActionResult<IEnumerable<Object2DDto>>> GetByEnvironmentId(int environmentId)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var objects = await _repository.GetAllByEnvironmentIdAsync(environmentId, userId);
        return Ok(objects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Object2DDto>> GetById(int id)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var obj = await _repository.GetByIdAsync(id, userId);
        if (obj == null)
            return NotFound();

        return Ok(obj);
    }

    [HttpPost]
    public async Task<ActionResult<Object2DDto>> Create(Object2DDto obj)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var created = await _repository.CreateAsync(obj, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("You don't have access to this environment.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Object2DDto obj)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (id != obj.Id)
            return BadRequest();

        var success = await _repository.UpdateAsync(obj, userId);
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
