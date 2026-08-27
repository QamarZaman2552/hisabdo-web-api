using HisabDo.API.Extensions;
using HisabDo.Application.DTOs;
using HisabDo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HisabDo.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TransactionsController(
    ITransactionService transactionService,
    IOptions<UploadSettings> uploadOptions,
    ILogger<TransactionsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<TransactionDto>>> GetAllTransactions([FromQuery] TransactionFilterDto filter)
    {
        return Ok(await transactionService.GetAllAsync(User.GetUserId(), filter));
    }

    [HttpGet("~/api/v1/Categories/{categoryId:int}/transactions")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByCategory(int categoryId)
    {
        return Ok(await transactionService.GetByCategoryAsync(User.GetUserId(), categoryId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransactionDto>> GetTransactionById(int id)
    {
        var transaction = await transactionService.GetByIdAsync(User.GetUserId(), id);

        if (transaction == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Resource not found",
                detail: $"No transaction found with ID: {id}", type: "https://tools.ietf.org/html/rfc9110#section-15.5.5");
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

    [HttpPost("{id:int}/attachment")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadAttachment(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Bad request",
                detail: "No file uploaded.", type: "https://tools.ietf.org/html/rfc9110#section-15.5.1");
        }

        var settings = uploadOptions.Value;
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!settings.AllowedExtensions.Contains(ext))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Bad request",
                detail: $"Only {string.Join(", ", settings.AllowedExtensions)} files are allowed.", type: "https://tools.ietf.org/html/rfc9110#section-15.5.1");
        }

        var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };
        if (!string.IsNullOrEmpty(file.ContentType) && !allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Bad request",
                detail: "File content type does not match the file extension.", type: "https://tools.ietf.org/html/rfc9110#section-15.5.1");
        }

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), settings.Directory);
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        var existingUrl = await transactionService.GetAttachmentUrlAsync(User.GetUserId(), id);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        try
        {
            var attachmentUrl = $"/uploads/{fileName}";
            await transactionService.UpdateAttachmentUrlAsync(User.GetUserId(), id, attachmentUrl);
        }
        catch
        {
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            throw;
        }

        if (!string.IsNullOrEmpty(existingUrl))
        {
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), settings.Directory, existingUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldFilePath))
            {
                try
                {
                    System.IO.File.Delete(oldFilePath);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to delete old attachment file: {Path}", oldFilePath);
                }
            }
        }

        return Ok(new { attachmentUrl = $"/uploads/{fileName}", fileName });
    }
}