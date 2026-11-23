using System.Globalization;
using System.Threading.RateLimiting;

namespace ClaimGuard.Api.Security;

public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimiting";

    /// <summary>Requests allowed per caller per window.</summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>Window length in seconds.</summary>
    public int WindowSeconds { get; set; } = 60;
}

public static class RateLimitingSetup
{
    /// <summary>
    /// Global fixed-window limiter partitioned by caller (OWASP API4: unrestricted resource consumption).
    /// Authenticated callers are keyed by user name; anonymous callers by IP address.
    /// </summary>
    public static IServiceCollection AddClaimGuardRateLimiting(this IServiceCollection services, IConfiguration config)
    {
        var options = config.GetSection(RateLimitOptions.SectionName).Get<RateLimitOptions>() ?? new RateLimitOptions();

        services.AddRateLimiter(limiter =>
        {
            limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            limiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var key = context.User.Identity?.IsAuthenticated == true
                    ? $"user:{context.User.Identity.Name}"
                    : $"ip:{context.Connection.RemoteIpAddress}";

                return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = options.PermitLimit,
                    Window = TimeSpan.FromSeconds(options.WindowSeconds),
                    QueueLimit = 0
                });
            });

            limiter.OnRejected = (ctx, _) =>
            {
                ctx.HttpContext.Response.Headers.RetryAfter = options.WindowSeconds.ToString(CultureInfo.InvariantCulture);
                return ValueTask.CompletedTask;
            };
        });

        return services;
    }
}
