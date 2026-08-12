using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HisabDo.Application.DTOs;
using HisabDo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HisabDo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        var response = await authService.RegisterAsync(registerDto);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var response = await authService.LoginAsync(loginDto);

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? "0");
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new { userId, email, role });
    }
}