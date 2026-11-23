using System.Net;
using System.Text.Json;

namespace ClaimGuard.IntegrationTests.Security;

/// <summary>
/// Proves the baseline controls from section 5 of the plan against the running pipeline.
/// </summary>
public class BaselineSecurityTests
{
    [Fact]
    public async Task Health_endpoint_returns_200()
    {
        await using var factory = new ApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Responses_carry_security_headers_and_no_server_banner() // API8
    {
        await using var factory = new ApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1");

        Assert.Equal("nosniff", Header(response, "X-Content-Type-Options"));
        Assert.Equal("DENY", Header(response, "X-Frame-Options"));
        Assert.Equal("no-store", Header(response, "Cache-Control"));
        Assert.StartsWith("default-src 'none'", Header(response, "Content-Security-Policy"));
        Assert.False(response.Headers.Contains("Server"));
    }

    [Fact]
    public async Task Unknown_route_returns_problem_details_without_internals() // API8
    {
        await using var factory = new ApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/does-not-exist");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var json = JsonDocument.Parse(body);
        Assert.Equal(404, json.RootElement.GetProperty("status").GetInt32());
        Assert.DoesNotContain("exception", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("stackTrace", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Caller_over_the_limit_gets_429_with_retry_after() // API4
    {
        const int limit = 5;
        await using var factory = new ApiFactory(permitLimit: limit);
        var client = factory.CreateClient();

        for (var i = 0; i < limit; i++)
        {
            var ok = await client.GetAsync("/api/v1");
            Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
        }

        var rejected = await client.GetAsync("/api/v1");

        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
        Assert.True(rejected.Headers.Contains("Retry-After"));
    }

    [Fact]
    public async Task Health_check_is_never_rate_limited()
    {
        await using var factory = new ApiFactory(permitLimit: 1);
        var client = factory.CreateClient();

        for (var i = 0; i < 5; i++)
        {
            var response = await client.GetAsync("/health");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    private static string Header(HttpResponseMessage response, string name) =>
        response.Headers.TryGetValues(name, out var values) ? string.Join(",", values)
        : response.Content.Headers.TryGetValues(name, out var contentValues) ? string.Join(",", contentValues)
        : "";
}
