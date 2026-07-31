using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
    public void AddConfiguredOpenApiViaSwagger_ShouldRegisterValidatedSettings_WhenConfigurationIsValid()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration);

        // Act
        builder.AddConfiguredOpenApiViaSwagger();
        using var app = builder.Build();

        // Assert
        var settings = app.Services.GetRequiredService<SwaggerAppSettings>();
        Assert.Equal("Title", settings.Title);
        Assert.Equal("1.0", settings.ApiVersion);
        Assert.Equal(["http://localhost:5001"], settings.Servers);
    }

    [Fact]
    public void AddConfiguredOpenApiViaSwagger_ShouldThrowArgumentException_WhenConfigurationIsInvalid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["SwaggerAppSettings:Title"] = "Title",
        });

        // Act & Assert
        // валидация выполняется в момент регистрации, а не при разрешении сервиса
        Assert.Throws<ArgumentException>(() => builder.AddConfiguredOpenApiViaSwagger());
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

        // SwaggerGen требует ApiExplorer; в development среде хост проверяет
        // граф зависимостей при Build(), поэтому регистрируем его как в реальном приложении
        builder.Services.AddEndpointsApiExplorer();

        return builder;
    }
}
