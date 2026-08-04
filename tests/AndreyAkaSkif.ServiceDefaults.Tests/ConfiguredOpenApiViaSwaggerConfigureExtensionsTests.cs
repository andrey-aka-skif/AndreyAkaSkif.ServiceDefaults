using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class ConfiguredOpenApiViaSwaggerConfigureExtensionsTests
{
    private static readonly Dictionary<string, string?> ValidConfiguration = new()
    {
        ["SwaggerAppSettings:Title"] = "Title",
        ["SwaggerAppSettings:Description"] = "Description",
        ["SwaggerAppSettings:ApiVersion"] = "1.0",
        ["SwaggerAppSettings:Servers:0"] = "http://localhost:5001",
    };

    [Fact]
    public void AddConfiguredOpenApiViaSwagger_ShouldBindSettings_WhenConfigurationIsValid()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration);

        // Act
        builder.AddConfiguredOpenApiViaSwagger();
        using var app = builder.Build();

        // Assert
        var settings = app.Services.GetRequiredService<IOptions<SwaggerAppSettings>>().Value;
        Assert.Equal("Title", settings.Title);
        Assert.Equal("1.0", settings.ApiVersion);
        Assert.Equal(["http://localhost:5001"], settings.Servers);
    }

    [Fact]
    public void AddConfiguredOpenApiViaSwagger_ShouldThrowOptionsValidationException_WhenConfigurationIsInvalid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["SwaggerAppSettings:Title"] = "Title",
        });

        // Act
        // регистрация не падает: валидация выполняется конвейером параметров
        builder.AddConfiguredOpenApiViaSwagger();
        using var app = builder.Build();

        // Assert
        Assert.Throws<OptionsValidationException>(
            () => app.Services.GetRequiredService<IOptions<SwaggerAppSettings>>().Value);
    }

    [Fact]
    public void AddConfiguredOpenApiViaSwagger_ShouldRegisterApiExplorer_WhenCalled()
    {
        // Arrange
        // SwaggerGenerator строится поверх ApiExplorer, а для minimal API тот не
        // регистрируется сам; в development среде хост проверяет граф зависимостей
        // при Build(), поэтому нехватка провайдера выявляется здесь же
        var builder = CreateBuilderWith(ValidConfiguration, Environments.Development);

        // Act
        builder.AddConfiguredOpenApiViaSwagger();
        using var app = builder.Build();

        // Assert
        Assert.NotNull(app.Services.GetRequiredService<ISwaggerProvider>());
    }

    [Fact]
    public void AddConfiguredOpenApiViaSwagger_ShouldUseRelativeServer_WhenServersAreNotConfigured()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["SwaggerAppSettings:Title"] = "Title",
            ["SwaggerAppSettings:Description"] = "Description",
            ["SwaggerAppSettings:ApiVersion"] = "1.0",
        });

        // Act
        builder.AddConfiguredOpenApiViaSwagger();
        using var app = builder.Build();

        // Assert
        var settings = app.Services.GetRequiredService<IOptions<SwaggerAppSettings>>().Value;
        Assert.Equal([SwaggerAppSettings.DefaultServer], settings.Servers);
    }

    [Fact]
    public void AddConfiguredOpenApiViaSwagger_ShouldDescribeApiFromSettings_WhenSpecificationIsGenerated()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration, Environments.Development);

        // Act
        builder.AddConfiguredOpenApiViaSwagger();
        using var app = builder.Build();

        // Assert
        // спецификация собирается лениво, поэтому проверяется результат, а не регистрация
        var document = app.Services.GetRequiredService<ISwaggerProvider>().GetSwagger("1.0");
        Assert.Equal("Title", document.Info.Title);
        Assert.Equal("Description", document.Info.Description);
        Assert.Equal(["http://localhost:5001"], document.Servers.Select(server => server.Url));
    }

    [Fact]
    public void UseConfiguredOpenApiViaSwagger_ShouldNotThrow_WhenAddWasCalled()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration, Environments.Development);
        builder.AddConfiguredOpenApiViaSwagger();
        using var app = builder.Build();

        // Act & Assert
        Assert.Same(app, app.UseConfiguredOpenApiViaSwagger());
    }

    [Fact]
    public void UseConfiguredOpenApiViaSwagger_ShouldThrowInvalidOperationException_WhenAddWasNotCalled()
    {
        // Arrange
        // конфигурация валидна, но парный Add* не вызван
        var builder = CreateBuilderWith(ValidConfiguration, Environments.Development);
        using var app = builder.Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => app.UseConfiguredOpenApiViaSwagger());
    }

    [Fact]
    public void UseConfiguredOpenApiViaSwagger_ShouldNotThrow_WhenEnvironmentIsNotDevelopment()
    {
        // Arrange
        // вне development метод не делает ничего и не требует настроек
        var builder = CreateBuilderWith(ValidConfiguration);
        using var app = builder.Build();

        // Act & Assert
        Assert.Same(app, app.UseConfiguredOpenApiViaSwagger());
    }

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
