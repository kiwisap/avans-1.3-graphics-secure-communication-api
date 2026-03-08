using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GraphicsSecureCommunicationApi.WebApi.Models.Dto;
using GraphicsSecureCommunicationApi.WebApi.Services.Interfaces;

namespace GraphicsSecureCommunicationApi.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnvironmentsController : ControllerBase
{
    private readonly IEnvironment2DService _service;
    private readonly IAuthenticationService _authService;

    public EnvironmentsController(IEnvironment2DService service, IAuthenticationService authService)
    {
        _service = service;
        _authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Environment2DDto>>> GetAll()
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var environments = await _service.GetAllByUserIdAsync(userId);
        return Ok(environments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Environment2DDto>> GetById(int id)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var environment = await _service.GetByIdAsync(id, userId);
        if (environment == null)
            return NotFound();

        return Ok(environment);
    }

    [HttpPost]
    public async Task<ActionResult<Environment2DDto>> Create(Environment2DDto environment)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var created = await _service.CreateAsync(environment, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Environment2DDto environment)
    {
        var userId = _authService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (id != environment.Id)
            return BadRequest("ID mismatch.");

        try
        {
            var success = await _service.UpdateAsync(environment, userId);
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
