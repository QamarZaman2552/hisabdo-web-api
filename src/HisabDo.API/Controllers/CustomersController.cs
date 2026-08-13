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
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
    {
        return Ok(await customerService.GetAllAsync(User.GetUserId()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(int id)
    {
        var customer = await customerService.GetByIdAsync(id);

        if (customer == null)
        {
            return NotFound(new { message = $"No customer found with ID: {id}" });
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
        var customer = await customerService.UpdateAsync(id, customerDto);

        return Ok(customer);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        await customerService.DeleteAsync(id);

        return NoContent();
    }
}