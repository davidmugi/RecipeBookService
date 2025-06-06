using RecipeBookService.Configurations;
using RecipeBookService.DTOs;
using RecipeBookService.Exceptions;

namespace RecipeBookService.Configurations.MiddleWares;

public class ExceptionHandlingMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,"An exception occurred : {Exception}", exception.Message);
            await HandleException(httpContext, exception);
        }
    }

    private static Task HandleException(HttpContext httpContext, Exception exception)
    {
        httpContext.Response.ContentType = "application/json";

        httpContext.Response.StatusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        var internalStatusCode = exception switch
        {
            UnauthorizedException => (int)InternalStatusCode.Unauthorized,
            NotFoundException => (int)InternalStatusCode.NotFound,
            UnauthorizedAccessException => (int)InternalStatusCode.Unauthorized,
            _ => (int)InternalStatusCode.InternalServerError
        };

        var message = exception switch
        {
            UnauthorizedException => exception.Message,
            NotFoundException => exception.Message,
            UnauthorizedAccessException => "Unauthorized access",
            _ => "Error occured while proccessing your request"
        };

        var response = new BaseDTO<LoginResponseDTO>(internalStatusCode, message, null);

        return httpContext.Response.WriteAsJsonAsync(response);
    }
}