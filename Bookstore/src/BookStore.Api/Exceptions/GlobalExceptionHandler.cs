using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Exceptions;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "An exception occurred while processing the request.");

        var statusCode = exception switch
        {
            BusinessRuleException =>
                StatusCodes.Status409Conflict,

            _ =>
                StatusCodes.Status500InternalServerError
        };

        var title = exception switch
        {
            BusinessRuleException =>
                "Business rule violation.",

            _ =>
                "An unexpected error occurred."
        };

        var detail = exception switch
        {
            BusinessRuleException =>
                exception.Message,

            _ =>
                "An unexpected error occurred while processing the request."
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}