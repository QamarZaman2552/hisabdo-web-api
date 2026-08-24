using HisabDo.API.Extensions;
using HisabDo.Application.DTOs;
using HisabDo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HisabDo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<CategoryDto>>> GetAllCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        return Ok(await categoryService.GetAllAsync(User.GetUserId(), page, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
    {
        var category = await categoryService.GetByIdAsync(User.GetUserId(), id);

        if (category == null)
        {
            return NotFound(new { message = $"No category found with ID: {id}" });
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> AddCategory([FromBody] CreateCategoryDto categoryDto)
    {
        var category = await categoryService.CreateAsync(User.GetUserId(), categoryDto);

        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto categoryDto)
    {
        var category = await categoryService.UpdateAsync(User.GetUserId(), id, categoryDto);

        return Ok(category);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await categoryService.DeleteAsync(User.GetUserId(), id);

        return NoContent();
    }
}
