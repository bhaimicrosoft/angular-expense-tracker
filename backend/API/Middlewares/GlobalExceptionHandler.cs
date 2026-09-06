using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Log the raw error for our internal server logs (so we can debug later)
        _logger.LogError(exception, $"An unhandled exception occurred : {exception.Message}");

        // 2. Setup a standard ProblemDetails response to send back to the client
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
        };

        // 3. Handle our specific Validation Exception safely
        if (exception is CustomValidationException validationException)
        {
            problemDetails.Title = "Validation Error";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = "One or more validation rules failed.";
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        // 4. Handle massive database crashes or unexpected bugs 
        else
        {
            problemDetails.Title = "Internal Server Error";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Detail = "An unexpected error occurred. Please try again later.";
        }

        // 5. Write the response back to the client
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);


        return true; // Tells the ASP.NET pipeline that we successfully handled the crash
    }
}