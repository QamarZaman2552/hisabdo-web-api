using HisabDo.Application.DTOs;
using HisabDo.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HisabDo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SettingsController(ISettingService settingService) : ControllerBase
{
    private const int CurrentUserId = 1;

    [HttpGet]
    public async Task<ActionResult<SettingDto>> GetSettings()
    {
        var settings = await settingService.GetAsync(CurrentUserId);

        if (settings == null)
        {
            return NotFound(new { message = $"No settings found for user ID: {CurrentUserId}" });
        }

        return Ok(settings);
    }

    [HttpPut]
    public async Task<ActionResult<SettingDto>> UpdateSettings([FromBody] UpdateSettingDto settingsDto)
    {
        var settings = await settingService.UpdateAsync(CurrentUserId, settingsDto);

        return Ok(settings);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteSettings()
    {
        await settingService.DeleteAsync(CurrentUserId);

        return NoContent();
    }
}