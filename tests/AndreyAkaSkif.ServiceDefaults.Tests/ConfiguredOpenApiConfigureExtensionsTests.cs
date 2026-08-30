using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
#if NET10_0_OR_GREATER
using Microsoft.AspNetCore.OpenApi;
#endif

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class ConfiguredOpenApiConfigureExtensionsTests
{
    private const string DocumentRoute = "/openapi/{documentName}.json";

    private static readonly Dictionary<string, string?> ValidConfiguration = new()
    {
        ["OpenApi:Title"] = "Title",
        ["OpenApi:Description"] = "Description",
        ["OpenApi:Version"] = "1.0",
        ["OpenApi:Servers:0"] = "/api",
    };

    [Fact]
    public void AddConfiguredOpenApi_ShouldBindSettings_WhenConfigurationIsValid()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration);

        // Act
        builder.AddConfiguredOpenApi();
        using var app = builder.Build();

        // Assert
        var settings = app.Services.GetRequiredService<IOptions<OpenApiAppSettings>>().Value;
        Assert.Equal("Title", settings.Title);
        Assert.Equal("Description", settings.Description);
        Assert.Equal("1.0", settings.Version);
        Assert.Equal(["/api"], settings.Servers);
    }

    [Fact]
    public void AddConfiguredOpenApi_ShouldUseDefaults_WhenOptionalKeysAreMissing()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["OpenApi:Title"] = "Title",
        });

        // Act
        builder.AddConfiguredOpenApi();
        using var app = builder.Build();

        // Assert
        var settings = app.Services.GetRequiredService<IOptions<OpenApiAppSettings>>().Value;
        Assert.Equal(OpenApiAppSettings.DefaultDocumentName, settings.DocumentName);
        Assert.Equal(OpenApiAppSettings.DefaultVersion, settings.Version);
        Assert.Equal([OpenApiAppSettings.DefaultServer], settings.Servers);
        Assert.Equal(OpenApiVisibility.ByEnvironment, settings.Visibility);
        Assert.Null(settings.Description);
    }

    [Fact]
    public void AddConfiguredOpenApi_ShouldThrowOptionsValidationException_WhenTitleIsMissing()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["OpenApi:Description"] = "Description",
        });

        // Act
        // регистрация не падает: валидация выполняется конвейером параметров
        builder.AddConfiguredOpenApi();
        using var app = builder.Build();

        // Assert
        Assert.Throws<OptionsValidationException>(
            () => app.Services.GetRequiredService<IOptions<OpenApiAppSettings>>().Value);
    }

    [Theory]
    [InlineData("Development", null, true)]
    [InlineData("Staging", null, true)]
    [InlineData("Production", null, false)]
    [InlineData("Production", "Always", true)]
    [InlineData("Development", "Never", false)]
    public void UseConfiguredOpenApi_ShouldMapDocument_AccordingToVisibility(
        string environmentName,
        string? visibility,
        bool expected)
    {
        // Arrange
        var configuration = new Dictionary<string, string?>(ValidConfiguration);

        if (visibility is not null)
            configuration["OpenApi:Visibility"] = visibility;

        var builder = CreateBuilderWith(configuration, environmentName);
        builder.AddConfiguredOpenApi();
        using var app = builder.Build();

        // Act
        app.UseConfiguredOpenApi();

        // Assert
        Assert.Equal(expected, RoutePatternsOf(app).Contains(DocumentRoute));
    }

    [Fact]
    public void UseConfiguredOpenApi_ShouldThrowInvalidOperationException_WhenAddWasNotCalled()
    {
        // Arrange
        // конфигурация валидна, но парный Add* не вызван
        var builder = CreateBuilderWith(ValidConfiguration, Environments.Development);
        using var app = builder.Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => app.UseConfiguredOpenApi());
    }

    [Theory]
    [InlineData("Development", true)]
    [InlineData("Production", false)]
    public void UseDefaultOpenApi_ShouldMapDocument_AccordingToEnvironment(
        string environmentName,
        bool expected)
    {
        // Arrange
        // пара без конфигурации: секции OpenApi нет вовсе
        var builder = CreateBuilderWith([], environmentName);
        builder.AddDefaultOpenApi();
        using var app = builder.Build();

        // Act
        app.UseDefaultOpenApi();

        // Assert
        Assert.Equal(expected, RoutePatternsOf(app).Contains(DocumentRoute));
    }

    [Fact]
    public void UseDefaultOpenApi_ShouldMapDocument_WhenVisibilityIsAlwaysInProduction()
    {
        // Arrange
        // ключ видимости читается и парой без конфигурации: секция необязательна,
        // но если задана — работает
        var builder = CreateBuilderWith(
            new Dictionary<string, string?> { ["OpenApi:Visibility"] = "Always" },
            Environments.Production);

        builder.AddDefaultOpenApi();
        using var app = builder.Build();

        // Act
        app.UseDefaultOpenApi();

        // Assert
        Assert.Contains(DocumentRoute, RoutePatternsOf(app));
    }

#if NET10_0_OR_GREATER
    [Fact]
    public async Task AddConfiguredOpenApi_ShouldDescribeApiFromSettings_WhenDocumentIsGenerated()
    {
        // Arrange
        // документ собирается провайдером спецификации: публичный API появился в .NET 10,
        // поэтому сквозная проверка живёт только на этом таргете
        var builder = CreateBuilderWith(ValidConfiguration, Environments.Development);
        builder.AddConfiguredOpenApi();
        builder.AddHealthCheckEndpointDescription();
        using var app = builder.Build();

        // Act
        var document = await app.Services
            .GetRequiredKeyedService<IOpenApiDocumentProvider>(OpenApiAppSettings.DefaultDocumentName)
            .GetOpenApiDocumentAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("Title", document.Info.Title);
        Assert.Equal("Description", document.Info.Description);
        Assert.Equal("1.0", document.Info.Version);
        Assert.Equal(["/api"], document.Servers!.Select(server => server.Url));
        Assert.Contains("/health", document.Paths);
    }

    [Fact]
    public async Task AddConfiguredOpenApi_ShouldUseConfiguredDocumentName_WhenItIsSet()
    {
        // Arrange
        var configuration = new Dictionary<string, string?>(ValidConfiguration)
        {
            ["OpenApi:DocumentName"] = "internal",
        };

        var builder = CreateBuilderWith(configuration, Environments.Development);
        builder.AddConfiguredOpenApi();
        using var app = builder.Build();

        // Act
        var document = await app.Services
            .GetRequiredKeyedService<IOpenApiDocumentProvider>("internal")
            .GetOpenApiDocumentAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("Title", document.Info.Title);
    }
#endif

    private static IReadOnlyList<string> RoutePatternsOf(WebApplication app)
        => [.. ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Select(endpoint => endpoint.RoutePattern.RawText ?? string.Empty)];

    private static WebApplicationBuilder CreateBuilderWith(
        Dictionary<string, string?> configuration,
        string? environmentName = null)
    {
        environmentName ??= Environments.Production;

        var builder = WebApplication.CreateSlimBuilder(
            new WebApplicationOptions { EnvironmentName = environmentName });
        builder.Configuration.AddInMemoryCollection(configuration);

        return builder;
    }
}
