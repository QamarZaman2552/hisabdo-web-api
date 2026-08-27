using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace HisabDo.API.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IWebHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Requested resource was not found.");
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, "Resource not found", ex.Message, "https://tools.ietf.org/html/rfc9110#section-15.5.5");
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation failed.");
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "Validation error", ex.Message, "https://tools.ietf.org/html/rfc9110#section-15.5.1");
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid operation in request.");
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "Bad request", ex.Message, "https://tools.ietf.org/html/rfc9110#section-15.5.1");
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Unauthorized access attempt.");
            await WriteProblemAsync(context, StatusCodes.Status401Unauthorized, "Unauthorized", ex.Message, "https://tools.ietf.org/html/rfc9110#section-15.5.2");
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Database constraint violation.");
            await WriteProblemAsync(context, StatusCodes.Status409Conflict, "Conflict", "The request conflicts with existing data.", "https://tools.ietf.org/html/rfc9110#section-15.5.10");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "Internal server error", "An unexpected error occurred. Please try again later.", "https://tools.ietf.org/html/rfc7231#section-6.6.1");
        }
    }

    private async Task WriteProblemAsync(HttpContext context, int statusCode, string title, string detail, string type)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json; charset=utf-8";

        var responseDetail = env.IsDevelopment() || statusCode < 500 ? detail : "An error occurred. Please try again later.";

        var problem = new
        {
            type,
            title,
            status = statusCode,
            detail = responseDetail,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}