using HisabDo.Application.DTOs;
using HisabDo.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HisabDo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    private const int CurrentUserId = 1;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactions()
    {
        return Ok(await transactionService.GetAllAsync(CurrentUserId));
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
        var transaction = await transactionService.CreateAsync(CurrentUserId, transactionDto);

        return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTransaction(int id, [FromBody] CreateTransactionDto transactionDto)
    {
        var transaction = await transactionService.UpdateAsync(id, transactionDto);

        return Ok(transaction);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        await transactionService.DeleteAsync(id);

        return NoContent();
    }
}