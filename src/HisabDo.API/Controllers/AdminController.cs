using HisabDo.Application.DTOs;
using HisabDo.Application.Services;
using HisabDo.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HisabDo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class AdminController(IUserService userService) : ControllerBase
{
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        return Ok(await userService.GetAllAsync());
    }
}