using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using CorsOptions = Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class ConfiguredCorsExtensionsTests
{
    private static readonly Dictionary<string, string?> ValidConfiguration = new()
    {
        ["CorsPolicy:Name"] = "PolicyName",
        ["CorsPolicy:Origins:0"] = "http://localhost:5001",
    };

    [Fact]
    public void AddConfiguredCorsPolicy_ShouldBindSettings_WhenConfigurationIsValid()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration);

        // Act
        builder.AddConfiguredCorsPolicy();
        using var app = builder.Build();

        // Assert
        var corsPolicy = app.Services.GetRequiredService<IOptions<CorsPolicy>>().Value;
        Assert.Equal("PolicyName", corsPolicy.Name);
        Assert.Equal(["http://localhost:5001"], corsPolicy.Origins);
    }

    [Fact]
    public void AddConfiguredCorsPolicy_ShouldRegisterPolicyUnderConfiguredName()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration);

        // Act
        builder.AddConfiguredCorsPolicy();
        using var app = builder.Build();

        // Assert
        var policy = app.Services.GetRequiredService<IOptions<CorsOptions>>().Value.GetPolicy("PolicyName");
        Assert.NotNull(policy);
        Assert.Equal(["http://localhost:5001"], policy.Origins);
    }

    [Fact]
    public void AddConfiguredCorsPolicy_ShouldThrowOptionsValidationException_WhenConfigurationIsInvalid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["CorsPolicy:Name"] = "PolicyName",
        });

        // Act
        // регистрация не падает: валидация выполняется конвейером параметров
        builder.AddConfiguredCorsPolicy();
        using var app = builder.Build();

        // Assert
        Assert.Throws<OptionsValidationException>(
            () => app.Services.GetRequiredService<IOptions<CorsPolicy>>().Value);
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
