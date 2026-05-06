using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities; 
using Microsoft.Data.SqlClient; 
using WarehouseServices.Exceptions;

namespace WarehouseAPI.Services;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            SqlException => StatusCodes.Status500InternalServerError, 
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;

        if (statusCode >= 500)
        {
            logger.LogError(exception, "A critical server or database error occurred: {Message}", exception.Message);
        }
        else
        {
            logger.LogWarning("A client error occurred: {Message}", exception.Message);
        }

        string errorMessage = exception switch
        {
            SqlException => "A database error occurred while processing your request. Please try again later.", 
            _ when statusCode >= 500 => "An unexpected server error occurred.",
            _ => exception.Message
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails()
            {
                Type = exception.GetType().Name,
                Title = ReasonPhrases.GetReasonPhrase(statusCode),
                Status = statusCode,
                Detail = errorMessage,
                Instance = httpContext.Request.Path
            }
        });
    }
}