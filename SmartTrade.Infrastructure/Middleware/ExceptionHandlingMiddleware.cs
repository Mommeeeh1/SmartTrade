using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SmartTrade.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass the request to the next middleware in the pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception with full details
            _logger.LogError(ex, 
                "An unhandled exception occurred. Request Path: {Path}, Method: {Method}", 
                context.Request.Path, 
                context.Request.Method);
            
            // Rethrow the exception so UseExceptionHandler can catch it and show friendly error page
            throw;
        }
    }
}

