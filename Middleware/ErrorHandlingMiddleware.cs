public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);

            // Etter pipeline – logg statuskoder
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                _logger.LogWarning("🔒 401 Unauthorized - Ikke autorisert forespørsel til: {Path}", context.Request.Path);
            }

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                _logger.LogWarning("🚫 403 Forbidden - Ingen tilgang til: {Path}", context.Request.Path);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Uventet feil oppstod!");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new
            {
                error = "Internal Server Error",
                message = ex.Message,
                //stackTrace = _env.IsDevelopment() ? ex.StackTrace : null
                fullException = ex.ToString()

            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
