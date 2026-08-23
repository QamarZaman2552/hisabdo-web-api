using HisabDo.API.Extensions;
using HisabDo.Application.DTOs;
using HisabDo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HisabDo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactions([FromQuery] TransactionFilterDto filter)
    {
        return Ok(await transactionService.GetAllAsync(User.GetUserId(), filter));
    }

    [HttpGet("~/api/v1/categories/{categoryId:int}/transactions")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByCategory(int categoryId)
    {
        return Ok(await transactionService.GetByCategoryAsync(User.GetUserId(), categoryId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransactionDto>> GetTransactionById(int id)
    {
        var transaction = await transactionService.GetByIdAsync(id);

        if (transaction == null)
        {
            return NotFound(new { message = $"No transaction found with ID: {id}" });
        }

        return Ok(transaction);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> AddTransaction([FromBody] CreateTransactionDto transactionDto)
    {
        var transaction = await transactionService.CreateAsync(User.GetUserId(), transactionDto);

        return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTransaction(int id, [FromBody] CreateTransactionDto transactionDto)
    {
        var transaction = await transactionService.UpdateAsync(User.GetUserId(), id, transactionDto);

        return Ok(transaction);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        await transactionService.DeleteAsync(User.GetUserId(), id);

        return NoContent();
    }
}