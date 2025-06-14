namespace MinAPI;

public class ApiKeyAuthenticationFilter
{
    private readonly RequestDelegate _next;

    public ApiKeyAuthenticationFilter(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var requestApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Missing API key");
            return;
        }
        
        if (requestApiKey!="GIGA-PIPI")
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API key");
            return;
        }

        await _next(context);
    }
}