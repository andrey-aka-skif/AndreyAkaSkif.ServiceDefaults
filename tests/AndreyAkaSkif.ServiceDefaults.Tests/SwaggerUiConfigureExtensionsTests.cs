using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class SwaggerUiConfigureExtensionsTests
{
    private const string RedirectRoute = "/";

    [Fact]
    public void AddSwaggerUi_ShouldBindSettings_WhenSectionIsConfigured()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["Swagger:Url"] = "/openapi/internal.json",
            ["Swagger:Name"] = "Demo API",
            ["Swagger:Visibility"] = "Always",
        });

        // Act
        builder.AddSwaggerUi();
        using var app = builder.Build();

        // Assert
        var settings = app.Services.GetRequiredService<IOptions<SwaggerAppSettings>>().Value;
        Assert.Equal("/openapi/internal.json", settings.Url);
        Assert.Equal("Demo API", settings.DisplayName);
        Assert.Equal(SwaggerUiVisibility.Always, settings.Visibility);
    }

    [Fact]
    public void AddSwaggerUi_ShouldUseDefaults_WhenSectionIsMissing()
    {
        // Arrange
        // секция необязательна целиком: пакет показывает документ по умолчанию
        var builder = CreateBuilderWith([]);

        // Act
        builder.AddSwaggerUi();
        using var app = builder.Build();

        // Assert
        var settings = app.Services.GetRequiredService<IOptions<SwaggerAppSettings>>().Value;
        Assert.Equal(SwaggerAppSettings.DefaultUrl, settings.Url);
        Assert.Equal(SwaggerAppSettings.DefaultUrl, settings.DisplayName);
        Assert.Equal(SwaggerUiVisibility.ByEnvironment, settings.Visibility);
    }

    [Fact]
    public void AddSwaggerUi_ShouldThrowOptionsValidationException_WhenUrlIsBlank()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["Swagger:Url"] = string.Empty,
        });

        // Act
        // регистрация не падает: валидация выполняется конвейером параметров
        builder.AddSwaggerUi();
        using var app = builder.Build();

        // Assert
        Assert.Throws<OptionsValidationException>(
            () => app.Services.GetRequiredService<IOptions<SwaggerAppSettings>>().Value);
    }

    [Theory]
    [InlineData("Development", null, true)]
    [InlineData("Staging", null, true)]
    [InlineData("Production", null, false)]
    [InlineData("Production", "Always", true)]
    [InlineData("Development", "Never", false)]
    public void UseSwaggerUi_ShouldShowUi_AccordingToVisibility(
        string environmentName,
        string? visibility,
        bool expected)
    {
        // Arrange
        var configuration = new Dictionary<string, string?>();

        if (visibility is not null)
            configuration["Swagger:Visibility"] = visibility;

        var builder = CreateBuilderWith(configuration, environmentName);
        builder.AddSwaggerUi();
        using var app = builder.Build();

        // Act
        app.UseSwaggerUi();

        // Assert
        // страница UI поднимается промежуточным ПО, поэтому наблюдаемый признак —
        // перенаправление с корня, которое ставится в паре с ней
        Assert.Equal(expected, RoutePatternsOf(app).Contains(RedirectRoute));
    }

    [Fact]
    public void UseSwaggerUi_ShouldThrowInvalidOperationException_WhenAddWasNotCalled()
    {
        // Arrange
        var builder = CreateBuilderWith([], Environments.Development);
        using var app = builder.Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => app.UseSwaggerUi());
    }

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
