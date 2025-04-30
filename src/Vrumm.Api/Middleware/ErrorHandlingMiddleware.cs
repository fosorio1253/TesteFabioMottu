using System.Net;
using System.Text.Json;
using Vrumm.Api.Models;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Domain.Exceptions.Drivers;
using Vrumm.Domain.Exceptions.Motorcycles;

namespace Vrumm.Api.Middleware;
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, errorResponse) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new ErrorResponse("Dados inválidos")
            ),
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                new ErrorResponse(notFoundEx.Message)
            ),
            DuplicateCnpjException => (
                HttpStatusCode.BadRequest,
                new ErrorResponse("Dados inválidos")
            ),
            DuplicateLicensePlateException => (
                HttpStatusCode.BadRequest,
                new ErrorResponse("Dados inválidos")
            ),
            MotorcycleHasRentalsException => (
                HttpStatusCode.BadRequest,
                new ErrorResponse("Dados inválidos")
            ),
            FileUploadException => (
                HttpStatusCode.BadRequest,
                new ErrorResponse("Dados inválidos")
            ),
            InvalidFileTypeException => (
                HttpStatusCode.BadRequest,
                new ErrorResponse("Dados inválidos")
            ),
            _ => (
                HttpStatusCode.BadRequest,
                new ErrorResponse("Dados inválidos")
            )
        };

        _logger.LogError(exception, "Error processing request: {Message}", exception.Message);
        response.StatusCode = (int)statusCode;

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}
