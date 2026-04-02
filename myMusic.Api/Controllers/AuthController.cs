using Microsoft.AspNetCore.Mvc;
using myMusic.Application.DTOs;
using myMusic.Application.Interfaces;

namespace myMusic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // ----------------------------------------------------------
    
    // REGISTER:
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);
        return Ok(result);
    }
    
    
    // LOGIN:
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
    
    
    // REFRESH:
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto refreshDto)
    {
        try
        {
            var result = await _authService.RefreshAsync(refreshDto);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Token de refresco inválido o expirado." });
        }
    }
    
    
    // REVOKE:
    [HttpPost("logout")]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenDto revokeTokenDto)
    {
        var result = await _authService.RevokeAsync(revokeTokenDto);
        if(!result) return NotFound(new { message = "Token no encontrado." });

        return Ok(new { message = "Sesión cerrada exitosamente" });
    }
}