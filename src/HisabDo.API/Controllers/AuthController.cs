using HisabDo.API.Extensions;
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
    public async Task<ActionResult<UserProfileDto>> Me()
    {
        return Ok(await authService.GetProfileAsync(User.GetUserId()));
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<UserProfileDto>> UpdateMe([FromBody] UpdateProfileDto updateProfileDto)
    {
        return Ok(await authService.UpdateProfileAsync(User.GetUserId(), updateProfileDto));
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        await authService.ChangePasswordAsync(User.GetUserId(), changePasswordDto);

        return Ok(new { message = "Password changed successfully." });
    }
}