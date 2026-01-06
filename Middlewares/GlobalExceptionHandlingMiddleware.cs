using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Global.App.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);   //pass the request to the next middleware in the pipeline
        }
        catch (DuplicateBookException ex)
        {
            _logger.LogError("Duplicate book ID exception.", ex);
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError("Unhandled exception occurred.", ex);
            await HandleExceptionAsync(httpContext, new Exception("An unexpected error occurred"));
        }
    }


    // This method formats and sends an error response based on the exception
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";  //set response type to " "
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = exception.Message;

        if (exception is DuplicateBookException)
        {
            statusCode = (int)HttpStatusCode.BadRequest;
            message = exception.Message;
        }

        // Writes an error message to the response using the ErrorDetails class
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(new ErrorDetails
        {
            StatusCode = statusCode,
            Message = message
        }.ToString());
    }
}

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }

    public override string ToString()
    {
        return $"Error: {StatusCode} - {Message}";
    }
}

public class DuplicateBookException : Exception
{
    public DuplicateBookException(string message) : base(message)
    {
    }
}
