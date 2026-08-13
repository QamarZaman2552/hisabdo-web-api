using HisabDo.API.Extensions;
using HisabDo.Application.DTOs;
using HisabDo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HisabDo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SettingsController(ISettingService settingService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SettingDto>> GetSettings()
    {
        var userId = User.GetUserId();
        var settings = await settingService.GetAsync(userId);

        if (settings == null)
        {
            return NotFound(new { message = $"No settings found for user ID: {userId}" });
        }

        return Ok(settings);
    }

    [HttpPut]
    public async Task<ActionResult<SettingDto>> UpdateSettings([FromBody] UpdateSettingDto settingsDto)
    {
        var settings = await settingService.UpdateAsync(User.GetUserId(), settingsDto);

        return Ok(settings);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteSettings()
    {
        await settingService.DeleteAsync(User.GetUserId());

        return NoContent();
    }
}