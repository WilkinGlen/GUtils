using FastEndpoints;

namespace GUtils.Api.Endpoints;

/// <summary>
/// Test endpoint to verify API functionality
/// </summary>
public class TestEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        var builder = SwaggerDescriptionBuilder.SwaggerDescriptionBuilder
            .Create()
            .WithTitle("Test Endpoint - Set in builder")
            .WithTag("Category", "Testing")
            .WithTag("Version", "1.0");

        this.Get("/test");
        this.AllowAnonymous();
        this.Summary(s =>
        {
            s.Summary = "Test endpoint";
            s.Description = builder.Build();
            s.Response<string>(200, "Returns a success message indicating the endpoint works");
            s.Responses[200] = "Test endpoint works!";
        });
        this.Description(d => d.Produces<string>(200, "text/plain"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await this.SendAsync("Test endpoint works!", cancellation: ct);
    }
}
