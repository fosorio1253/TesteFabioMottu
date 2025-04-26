using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Vrumm.Domain.Common.Exceptions;

namespace Vrumm.Infrastructure.Middleware;
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = GetErrorDetails(exception);
        context.Response.StatusCode = (int)statusCode;

        var errorResponse = new
        {
            statusCode,
            message
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, jsonOptions));
    }

    private static (HttpStatusCode statusCode, string message) GetErrorDetails(Exception exception)
    {
        return exception switch
        {
            InvalidLicensePlateException _ => (HttpStatusCode.BadRequest, exception.Message),
            MotorcycleNotAvailableException _ => (HttpStatusCode.Conflict, exception.Message),
            InvalidDriverLicenseException _ => (HttpStatusCode.BadRequest, exception.Message),
            DomainException _ => (HttpStatusCode.BadRequest, exception.Message),

            ArgumentException _ => (HttpStatusCode.BadRequest, exception.Message),
            InvalidOperationException _ => (HttpStatusCode.BadRequest, exception.Message),

            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
        };
    }
}