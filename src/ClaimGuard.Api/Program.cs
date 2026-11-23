using ClaimGuard.Api.Security;

var builder = WebApplication.CreateBuilder(args);

// Don't advertise the web server (OWASP API8).
builder.WebHost.ConfigureKestrel(kestrel => kestrel.AddServerHeader = false);

// --- Services -------------------------------------------------------------

// RFC 7807 ProblemDetails for every error response. No stack traces leave the API (OWASP API8).
builder.Services.AddProblemDetails();

// OpenAPI document at /openapi/v1.json (Development only). Swagger UI arrives in Sprint 1.
builder.Services.AddOpenApi();

// Per-caller rate limiting (OWASP API4). Limits come from configuration so tests can lower them.
builder.Services.AddClaimGuardRateLimiting(builder.Configuration);

// Liveness check used by Docker and Azure.
builder.Services.AddHealthChecks();

var app = builder.Build();

// --- Middleware pipeline (order matters) ----------------------------------

app.UseExceptionHandler();          // unhandled exceptions -> generic 500 ProblemDetails
app.UseStatusCodePages();           // bare 4xx/5xx -> ProblemDetails body

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    // TLS terminates at the Azure ingress; HSTS tells browsers to stay on HTTPS.
    app.UseHsts();
}

app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseRateLimiter();

// Sprint 2 adds: app.UseAuthentication(); app.UseAuthorization();

// --- Endpoints ------------------------------------------------------------

app.MapHealthChecks("/health").DisableRateLimiting();

app.MapGet("/api/v1", () => Results.Ok(new
    {
        name = "ClaimGuard API",
        version = "v1",
        status = "Sprint 0 scaffold"
    }))
    .WithName("GetApiInfo");

app.Run();

// Exposes Program to WebApplicationFactory in the integration tests.
public partial class Program;
