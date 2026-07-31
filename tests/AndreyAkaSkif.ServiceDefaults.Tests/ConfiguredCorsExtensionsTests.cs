using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class ConfiguredCorsExtensionsTests
{
    private static readonly Dictionary<string, string?> ValidConfiguration = new()
    {
        ["CorsPolicy:Name"] = "PolicyName",
        ["CorsPolicy:Origins:0"] = "http://localhost:5001",
    };

    [Fact]
    public void AddConfiguredCorsPolicy_ShouldRegisterValidatedSettings_WhenConfigurationIsValid()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration);

        // Act
        builder.AddConfiguredCorsPolicy();
        using var app = builder.Build();

        // Assert
        var corsPolicy = app.Services.GetRequiredService<CorsPolicy>();
        Assert.Equal("PolicyName", corsPolicy.Name);
        Assert.Equal(["http://localhost:5001"], corsPolicy.Origins);
    }

    [Fact]
    public void AddConfiguredCorsPolicy_ShouldThrowArgumentException_WhenConfigurationIsInvalid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["CorsPolicy:Name"] = "PolicyName",
        });

        // Act & Assert
        // валидация выполняется в момент регистрации, а не при разрешении сервиса
        Assert.Throws<ArgumentException>(() => builder.AddConfiguredCorsPolicy());
    }

    [Fact]
    public void UseConfiguredCorsPolicy_ShouldNotThrow_WhenAddWasCalled()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration);
        builder.AddConfiguredCorsPolicy();
        using var app = builder.Build();

        // Act & Assert
        Assert.Same(app, app.UseConfiguredCorsPolicy());
    }

    [Fact]
    public void UseConfiguredCorsPolicy_ShouldThrowInvalidOperationException_WhenAddWasNotCalled()
    {
        // Arrange
        // конфигурация валидна, но парный Add* не вызван
        var builder = CreateBuilderWith(ValidConfiguration);
        using var app = builder.Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => app.UseConfiguredCorsPolicy());
    }

    private static WebApplicationBuilder CreateBuilderWith(Dictionary<string, string?> configuration)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.Configuration.AddInMemoryCollection(configuration);

        return builder;
    }
}
