using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;
using Vrumm.Domain.Exceptions.Motorcycles;
using Vrumm.Domain.Exceptions;
using Vrumm.Infrastructure.Middleware;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Middleware;
public class GlobalExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionHandlingMiddleware>> _loggerMock;
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly GlobalExceptionHandlingMiddleware _middleware;

    public GlobalExceptionHandlingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
        _nextMock = new Mock<RequestDelegate>();
        _middleware = new GlobalExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_WhenDomainException_ReturnsBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new DomainException("Invalid operation");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var response = await GetResponse<ErrorResponse>(responseStream);
        response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        response.Message.Should().Be("Invalid operation");
    }

    [Fact]
    public async Task InvokeAsync_WhenInvalidLicensePlateException_ReturnsBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new InvalidLicensePlateException("Formato de placa inválido");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var response = await GetResponse<ErrorResponse>(responseStream);
        response.Message.Should().Be("Formato de placa inválido");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnexpectedException_ReturnsInternalServerError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new Exception("Unexpected error");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        var response = await GetResponse<ErrorResponse>(responseStream);
        response.Message.Should().Be("An unexpected error occurred. Please try again later.");
    }

    private async Task<T> GetResponse<T>(MemoryStream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        return await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private record ErrorResponse(int StatusCode, string Message);
}