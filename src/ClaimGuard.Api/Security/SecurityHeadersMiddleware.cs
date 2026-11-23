namespace ClaimGuard.Api.Security;

/// <summary>
/// Adds defensive HTTP response headers to every response (OWASP API8: security misconfiguration).
/// This is a JSON API, so the Content-Security-Policy is locked down completely.
/// </summary>
public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    public static readonly IReadOnlyDictionary<string, string> Headers = new Dictionary<string, string>
    {
        ["X-Content-Type-Options"] = "nosniff",
        ["X-Frame-Options"] = "DENY",
        ["Referrer-Policy"] = "no-referrer",
        ["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'",
        ["Cross-Origin-Resource-Policy"] = "same-origin",
        ["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()",
        ["Cache-Control"] = "no-store"
    };

    public Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            foreach (var (name, value) in Headers)
            {
                context.Response.Headers[name] = value;
            }

            // Don't advertise the server technology.
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");
            return Task.CompletedTask;
        });

        return next(context);
    }
}
