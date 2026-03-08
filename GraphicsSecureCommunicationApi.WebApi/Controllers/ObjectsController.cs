using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GraphicsSecureCommunicationApi.WebApi.Models.Dto;
using GraphicsSecureCommunicationApi.WebApi.Services.Interfaces;

namespace GraphicsSecureCommunicationApi.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ObjectsController : ControllerBase
{
    private readonly IObject2DService _service;
    private readonly IAuthenticationService _authService;

    public ObjectsController(IObject2DService service, IAuthenticationService authService)
    {
        _service = service;
        _authService = authService;
    }

    [HttpGet("environment/{environmentId}")]
    public async Task<ActionResult<IEnumerable<Object2DDto>>> GetByEnvironmentId(int environmentId)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var objects = await _service.GetAllByEnvironmentIdAsync(environmentId, userId);
        return Ok(objects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Object2DDto>> GetById(int id)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var obj = await _service.GetByIdAsync(id, userId);
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
            var created = await _service.CreateAsync(obj, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Object2DDto obj)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (id != obj.Id)
            return BadRequest("ID mismatch.");

        try
        {
            var success = await _service.UpdateAsync(obj, userId);
            if (!success)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var success = await _service.DeleteAsync(id, userId);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
