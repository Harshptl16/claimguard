using ClaimGuard.Api.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace ClaimGuard.UnitTests.Security;

public class SecurityHeadersMiddlewareTests
{
    [Fact]
    public async Task Adds_every_security_header_when_the_response_starts()
    {
        var (context, feature) = CreateContext();
        var middleware = new SecurityHeadersMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);
        await feature.StartAsync();

        foreach (var (name, value) in SecurityHeadersMiddleware.Headers)
        {
            Assert.Equal(value, context.Response.Headers[name].ToString());
        }
    }

    [Fact]
    public async Task Removes_headers_that_reveal_the_server_technology()
    {
        var (context, feature) = CreateContext();
        var middleware = new SecurityHeadersMiddleware(ctx =>
        {
            ctx.Response.Headers["Server"] = "Kestrel";
            ctx.Response.Headers["X-Powered-By"] = "ASP.NET";
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);
        await feature.StartAsync();

        Assert.False(context.Response.Headers.ContainsKey("Server"));
        Assert.False(context.Response.Headers.ContainsKey("X-Powered-By"));
    }

    [Fact]
    public void Content_security_policy_blocks_all_content_because_this_is_a_json_api()
    {
        Assert.Equal("default-src 'none'; frame-ancestors 'none'",
            SecurityHeadersMiddleware.Headers["Content-Security-Policy"]);
    }

    private static (HttpContext Context, CapturingResponseFeature Feature) CreateContext()
    {
        var feature = new CapturingResponseFeature();
        var context = new DefaultHttpContext();
        context.Features.Set<IHttpResponseFeature>(feature);
        return (context, feature);
    }

    /// <summary>Captures OnStarting callbacks so the test can fire them like Kestrel would.</summary>
    private sealed class CapturingResponseFeature : HttpResponseFeature
    {
        private readonly List<(Func<object, Task> Callback, object State)> _callbacks = [];

        public override void OnStarting(Func<object, Task> callback, object state) => _callbacks.Add((callback, state));

        public async Task StartAsync()
        {
            // Kestrel runs OnStarting callbacks in reverse registration order.
            for (var i = _callbacks.Count - 1; i >= 0; i--)
            {
                await _callbacks[i].Callback(_callbacks[i].State);
            }
        }
    }
}
