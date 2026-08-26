using HisabDo.API.Extensions;
using HisabDo.Application.DTOs;
using HisabDo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HisabDo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CustomersController(ICustomerService customerService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<CustomerDto>>> GetAllCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        return Ok(await customerService.GetAllAsync(User.GetUserId(), page, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(int id)
    {
        var customer = await customerService.GetByIdAsync(User.GetUserId(), id);

        if (customer == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Resource not found",
                detail: $"No customer found with ID: {id}", type: "https://tools.ietf.org/html/rfc9110#section-15.5.5");
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> AddCustomer([FromBody] CreateCustomerDto customerDto)
    {
        var customer = await customerService.CreateAsync(User.GetUserId(), customerDto);

        return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CreateCustomerDto customerDto)
    {
        var customer = await customerService.UpdateAsync(User.GetUserId(), id, customerDto);

        return Ok(customer);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        await customerService.DeleteAsync(User.GetUserId(), id);

        return NoContent();
    }
}