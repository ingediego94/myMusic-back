using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace myMusic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    // ping-pong para todos los usuarios:
    [Authorize]
    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        return Ok(new { message = "pong", timestamp = DateTime.UtcNow });
    }
    
    
    // ping-pong para administradores:
    [Authorize(Roles = "Admin")]
    [HttpGet("ping-admin")]
    public async Task<IActionResult> PingAdmin()
    {
        return Ok(new { message = "Pong exclusivo para usuarios administradores." });
    }
    
}