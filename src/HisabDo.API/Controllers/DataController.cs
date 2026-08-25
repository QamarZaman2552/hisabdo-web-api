using HisabDo.API.Extensions;
using HisabDo.Application.DTOs;
using HisabDo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HisabDo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DataController(IDataService dataService) : ControllerBase
{
    [HttpGet("backup")]
    public async Task<ActionResult<BackupFileDto>> Export()
    {
        return Ok(await dataService.ExportAsync(User.GetUserId()));
    }

    [HttpPost("restore")]
    public async Task<ActionResult<RestoreResultDto>> Restore(
        [FromBody] BackupFileDto data,
        [FromQuery] bool replace = false)
    {
        return Ok(await dataService.RestoreAsync(User.GetUserId(), data, replace));
    }

    [HttpDelete("all")]
    public async Task<IActionResult> ClearAll()
    {
        await dataService.ClearAllAsync(User.GetUserId());
        return NoContent();
    }
}
