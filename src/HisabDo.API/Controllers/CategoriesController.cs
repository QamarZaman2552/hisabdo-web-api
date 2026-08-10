using HisabDo.Application.DTOs;
using HisabDo.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HisabDo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    private const int CurrentUserId = 1;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
    {
        return Ok(await categoryService.GetAllAsync(CurrentUserId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
    {
        var category = await categoryService.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound(new { message = $"No category found with ID: {id}" });
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> AddCategory([FromBody] CreateCategoryDto categoryDto)
    {
        var category = await categoryService.CreateAsync(CurrentUserId, categoryDto);

        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto categoryDto)
    {
        var category = await categoryService.UpdateAsync(id, categoryDto);

        return Ok(category);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await categoryService.DeleteAsync(id);

        return NoContent();
    }
}
