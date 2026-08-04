using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class PathBaseExtensionsTests
{
    [Fact]
    public void AddConfiguredPathBase_ShouldBindSettings_WhenConfigurationIsValid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["PathBaseAppSettings:Path"] = "/api",
        });

        // Act
        builder.AddConfiguredPathBase();
        using var app = builder.Build();

        // Assert
        var settings = app.Services.GetRequiredService<IOptions<PathBaseAppSettings>>().Value;
        Assert.Equal("/api", settings.Path);
    }

    [Fact]
    public void AddConfiguredPathBase_ShouldThrowOptionsValidationException_WhenPathIsInvalid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["PathBaseAppSettings:Path"] = "api",
        });

        // Act
        // регистрация не падает: валидация выполняется конвейером параметров
        builder.AddConfiguredPathBase();
        using var app = builder.Build();

        // Assert
        var exception = Assert.Throws<OptionsValidationException>(
            () => app.Services.GetRequiredService<IOptions<PathBaseAppSettings>>().Value);
        Assert.Contains(nameof(PathBaseAppSettings.Path), exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UseConfiguredPathBase_ShouldNotThrow_WhenConfigurationIsValid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["PathBaseAppSettings:Path"] = "/api",
        });
        builder.AddConfiguredPathBase();
        using var app = builder.Build();

        // Act & Assert
        Assert.Same(app, app.UseConfiguredPathBase());
    }

    [Fact]
    public void UseConfiguredPathBase_ShouldNotThrow_WhenSectionIsMissing()
    {
        // Arrange
        // секции нет — базовый путь пустой, это допустимо
        var builder = CreateBuilderWith([]);
        builder.AddConfiguredPathBase();
        using var app = builder.Build();

        // Act & Assert
        Assert.Same(app, app.UseConfiguredPathBase());
    }

    [Fact]
    public void UseConfiguredPathBase_ShouldThrowInvalidOperationException_WhenAddWasNotCalled()
    {
        // Arrange
        // конфигурация валидна, но парный Add* не вызван
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["PathBaseAppSettings:Path"] = "/api",
        });
        using var app = builder.Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => app.UseConfiguredPathBase());
    }

    private static WebApplicationBuilder CreateBuilderWith(Dictionary<string, string?> configuration)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.Configuration.AddInMemoryCollection(configuration);

        return builder;
    }
}
