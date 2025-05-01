using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Domain.Exceptions;
using Vrumm.Infrastructure.Middleware;
using Vrumm.Test.Unit.Infrastructure.Storage;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Middleware;
public class CorrelationIdMiddlewareTests
{
    private readonly Mock<ILogger<CorrelationIdMiddleware>> _loggerMock;
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly CorrelationIdMiddleware _middleware;

    public CorrelationIdMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<CorrelationIdMiddleware>>();
        _nextMock = new Mock<RequestDelegate>();
        _middleware = new CorrelationIdMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    /*
    [Fact]
    public async Task InvokeAsync_WithCorrelationId_PropagatesHeader()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var correlationId = Guid.NewGuid().ToString();
        context.Request.Headers["X-Correlation-ID"] = correlationId;

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers["X-Correlation-ID"].Should()
            .ContainSingle().Which.Should().Be(correlationId);
        
        _loggerMock.VerifyLog(LogLevel.Information, $"Request {context.Request.Method} {context.Request.Path} started");
    }
    */

    [Fact]
    public async Task InvokeAsync_NoCorrelationId_GeneratesNewId()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers["X-Correlation-ID"].Should().ContainSingle();
        Guid.TryParse(context.Response.Headers["X-Correlation-ID"], out _).Should().BeTrue();
    }

    /*
    [Fact]
    public async Task InvokeAsync_WhenNextThrows_LogsError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var exception = new DomainException("Test error");
        _nextMock.Setup(n => n(context)).ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _middleware.InvokeAsync(context));
        _loggerMock.VerifyLog(LogLevel.Error, $"Request {context.Request.Method} {context.Request.Path} failed");
    }
    */
}