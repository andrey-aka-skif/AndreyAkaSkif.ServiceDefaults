using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
#if NET10_0_OR_GREATER
using Microsoft.OpenApi;
#else
using Microsoft.OpenApi.Models;
#endif

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class OpenApiDocumentTransformersTests
{
    [Fact]
    public async Task ApiInfoDocumentTransformer_ShouldDescribeApi_WhenSettingsAreConfigured()
    {
        // Arrange
        var settings = new OpenApiAppSettings
        {
            Title = "Title",
            Description = "Description",
            Version = "1.0.0-preview.1",
            Servers = ["/", "/api"],
        };

        var document = new OpenApiDocument();

        // Act
        await new ApiInfoDocumentTransformer(Options.Create(settings))
            .TransformAsync(document, CreateContext(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("Title", document.Info.Title);
        Assert.Equal("Description", document.Info.Description);
        Assert.Equal("1.0.0-preview.1", document.Info.Version);
        Assert.Equal(["/", "/api"], document.Servers!.Select(server => server.Url));
    }

    [Fact]
    public async Task HealthCheckDocumentTransformer_ShouldDescribeEndpoint_WhenApplied()
    {
        // Arrange
        var document = new OpenApiDocument();

        // Act
        await new HealthCheckDocumentTransformer("/health")
            .TransformAsync(document, CreateContext(), TestContext.Current.CancellationToken);

        // Assert
        var path = Assert.Contains("/health", document.Paths);
        var operation = GetOperation(path);

        Assert.Equal("Health Check", operation.Summary);

        var response = Assert.Contains("200", operation.Responses!);
        Assert.Equal("Healthy", response.Description);
        Assert.Contains("text/plain", response.Content!);
    }

    [Fact]
    public async Task HealthCheckDocumentTransformer_ShouldUseGivenAddress_WhenEndpointIsCustom()
    {
        // Arrange
        // адрес задаётся вызывающим: пакет не зависит от базового и константу
        // MapHealthCheckEndpoint() не видит
        var document = new OpenApiDocument();

        // Act
        await new HealthCheckDocumentTransformer("/api/health")
            .TransformAsync(document, CreateContext(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains("/api/health", document.Paths);
    }

    private static OpenApiDocumentTransformerContext CreateContext()
        => new()
        {
            DocumentName = OpenApiAppSettings.DefaultDocumentName,
            ApplicationServices = new ServiceCollection().BuildServiceProvider(),
            DescriptionGroups = [],
        };

#if NET10_0_OR_GREATER
    private static OpenApiOperation GetOperation(IOpenApiPathItem path)
        => path.Operations![HttpMethod.Get];
#else
    private static OpenApiOperation GetOperation(OpenApiPathItem path)
        => path.Operations[OperationType.Get];
#endif
}
