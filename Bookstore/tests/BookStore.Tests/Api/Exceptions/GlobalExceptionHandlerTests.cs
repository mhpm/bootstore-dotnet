using System.Text.Json;
using BookStore.Api.Exceptions;
using BookStore.Application.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookStore.Tests.Api.Exceptions;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
    private readonly GlobalExceptionHandler _handler;

    public GlobalExceptionHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        _handler = new GlobalExceptionHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnTrueAndSetConflictStatus_WhenBusinessRuleExceptionIsThrown()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/books/1";
        var responseBodyStream = new MemoryStream();
        httpContext.Response.Body = responseBodyStream;

        var exception = new BusinessRuleException("Cannot delete a loaned book.");

        // Act
        var handled = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        handled.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            responseBodyStream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(StatusCodes.Status409Conflict);
        problemDetails.Title.Should().Be("Business rule violation.");
        problemDetails.Detail.Should().Be("Cannot delete a loaned book.");
        problemDetails.Instance.Should().Be("/api/books/1");
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnTrueAndSetInternalServerError_WhenGenericExceptionIsThrown()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/books";
        var responseBodyStream = new MemoryStream();
        httpContext.Response.Body = responseBodyStream;

        var exception = new InvalidOperationException("Database connection timeout.");

        // Act
        var handled = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        handled.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            responseBodyStream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(StatusCodes.Status500InternalServerError);
        problemDetails.Title.Should().Be("An unexpected error occurred.");
        problemDetails.Detail.Should().Be("An unexpected error occurred while processing the request.");
        problemDetails.Instance.Should().Be("/api/books");
    }

    [Fact]
    public async Task TryHandleAsync_ShouldLogException()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        var exception = new Exception("Critical system error.");

        // Act
        await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
