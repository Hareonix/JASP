using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SingBoxController : ControllerBase
{
    private readonly ISingBoxService _service;

    public SingBoxController(ISingBoxService service)
    {
        _service = service;
    }

    [HttpGet("status")]
    public IActionResult Status() => Ok(new { running = _service.IsRunning });

    [HttpPost("start")]
    public async Task<IActionResult> Start()
    {
        await _service.StartAsync();
        return NoContent();
    }

    [HttpPost("stop")]
    public async Task<IActionResult> Stop()
    {
        await _service.StopAsync();
        return NoContent();
    }

    [HttpPost("restart")]
    public async Task<IActionResult> Restart()
    {
        await _service.RestartAsync();
        return NoContent();
    }
}
