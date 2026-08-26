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
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Resource not found",
                detail: "No settings found. Update settings to create them.", type: "https://tools.ietf.org/html/rfc9110#section-15.5.5");
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