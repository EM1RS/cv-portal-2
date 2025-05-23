public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); 
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { error = "Access denied" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "Internal Server Error", message = ex.Message });
        }

        // Hvis autorisasjon allerede ble avvist av ASP.NET (ikke exception)
        if (context.Response.StatusCode == 403)
        {
            await context.Response.WriteAsJsonAsync(new { error = "Access denied" });
        }
        else if (context.Response.StatusCode == 401)
        {
            await context.Response.WriteAsJsonAsync(new { error = "Not authenticated" });
        }
    }
}
