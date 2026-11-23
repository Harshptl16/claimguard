using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClaimGuard.IntegrationTests;

/// <summary>
/// Boots the real API in memory. Each test gets a fresh factory so rate-limit counters don't leak between tests.
/// Sprint 1 adds a Testcontainers PostgreSQL instance here.
/// </summary>
public sealed class ApiFactory(int permitLimit = 100) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("RateLimiting:PermitLimit", permitLimit.ToString(CultureInfo.InvariantCulture));
        builder.UseSetting("RateLimiting:WindowSeconds", "60");
    }
}
