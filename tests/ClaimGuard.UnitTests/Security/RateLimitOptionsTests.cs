using ClaimGuard.Api.Security;
using Microsoft.Extensions.Configuration;

namespace ClaimGuard.UnitTests.Security;

public class RateLimitOptionsTests
{
    [Fact]
    public void Defaults_to_100_requests_per_minute()
    {
        var options = new RateLimitOptions();

        Assert.Equal(100, options.PermitLimit);
        Assert.Equal(60, options.WindowSeconds);
    }

    [Fact]
    public void Binds_limits_from_configuration()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RateLimiting:PermitLimit"] = "5",
                ["RateLimiting:WindowSeconds"] = "10"
            })
            .Build();

        var options = config.GetSection(RateLimitOptions.SectionName).Get<RateLimitOptions>();

        Assert.NotNull(options);
        Assert.Equal(5, options.PermitLimit);
        Assert.Equal(10, options.WindowSeconds);
    }
}
