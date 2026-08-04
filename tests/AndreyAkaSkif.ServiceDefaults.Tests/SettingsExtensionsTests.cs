using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class SettingsExtensionsTests
{
    [Fact]
    public async Task AddValidatedOptions_ShouldBindSectionNamedAfterType()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestOptions:RequiredValue"] = "value",
            ["TestOptions:OtherRequiredValue"] = "other",
        });

        // Act
        builder.AddValidatedOptions<TestOptions, TestOptionsValidator>();
        using var host = builder.Build();
        await host.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        var options = host.Services.GetRequiredService<IOptions<TestOptions>>();
        Assert.Equal("value", options.Value.RequiredValue);

        await host.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task AddValidatedOptions_ShouldBindGivenSection_WhenSectionNameIsSpecified()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["Custom:RequiredValue"] = "value",
            ["Custom:OtherRequiredValue"] = "other",
        });

        // Act
        builder.AddValidatedOptions<TestOptions, TestOptionsValidator>("Custom");
        using var host = builder.Build();
        await host.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        var options = host.Services.GetRequiredService<IOptions<TestOptions>>();
        Assert.Equal("value", options.Value.RequiredValue);

        await host.StopAsync(TestContext.Current.CancellationToken);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void AddValidatedOptions_ShouldThrowArgumentException_WhenSectionNameIsBlank(string sectionName)
    {
        // Arrange
        var builder = CreateBuilderWith([]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => builder.AddValidatedOptions<TestOptions, TestOptionsValidator>(sectionName));
        Assert.Equal("sectionName", exception.ParamName);
    }

    [Fact]
    public async Task AddValidatedOptions_ShouldThrowOptionsValidationException_WhenConfigurationIsInvalid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestOptions:RequiredValue"] = string.Empty,
            ["TestOptions:OtherRequiredValue"] = "other",
        });

        // Act
        // регистрация не падает: валидация выполняется на старте хоста
        builder.AddValidatedOptions<TestOptions, TestOptionsValidator>();
        using var host = builder.Build();

        // Assert
        var exception = await Assert.ThrowsAsync<OptionsValidationException>(
            () => host.StartAsync(TestContext.Current.CancellationToken));
        Assert.Contains(
            $"Не задано значение {nameof(TestOptions)}:{nameof(TestOptions.RequiredValue)}",
            exception.Message);
    }

    [Fact]
    public async Task AddValidatedOptions_ShouldReportAllFailures_WhenSeveralRulesAreViolated()
    {
        // Arrange
        // обе обязательные величины пусты — конвейер отдаёт список сообщений,
        // а не первое попавшееся
        var builder = CreateBuilderWith([]);

        // Act
        builder.AddValidatedOptions<TestOptions, TestOptionsValidator>();
        using var host = builder.Build();

        // Assert
        var exception = await Assert.ThrowsAsync<OptionsValidationException>(
            () => host.StartAsync(TestContext.Current.CancellationToken));
        Assert.Collection(
            exception.Failures,
            failure => Assert.Contains(nameof(TestOptions.RequiredValue), failure),
            failure => Assert.Contains(nameof(TestOptions.OtherRequiredValue), failure));
    }

    [Fact]
    public void AddValidatedOptions_ShouldNotRegisterValue()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestOptions:RequiredValue"] = "value",
            ["TestOptions:OtherRequiredValue"] = "other",
        });

        // Act
        builder.AddValidatedOptions<TestOptions, TestOptionsValidator>();
        using var host = builder.Build();

        // Assert
        // инфраструктурный вариант: настройки доступны только как IOptions<T>
        Assert.Null(host.Services.GetService<TestOptions>());
    }

    [Fact]
    public void AddValidatedOptions_ShouldRegisterValidatorOnce_WhenCalledTwice()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestOptions:RequiredValue"] = "value",
            ["TestOptions:OtherRequiredValue"] = "other",
        });

        // Act
        builder.AddValidatedOptions<TestOptions, TestOptionsValidator>();
        builder.AddValidatedOptions<TestOptions, TestOptionsValidator>();
        using var host = builder.Build();

        // Assert
        Assert.Single(host.Services.GetServices<IValidateOptions<TestOptions>>());
    }

    [Fact]
    public void AddValidatedOptions_ShouldReturnSameBuilder()
    {
        // Arrange
        var builder = CreateBuilderWith([]);

        // Act
        var result = builder.AddValidatedOptions<TestOptions, TestOptionsValidator>();

        // Assert
        Assert.Same(builder, result);
    }

    [Fact]
    public async Task AddAppSettings_ShouldRegisterValue()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestOptions:RequiredValue"] = "value",
            ["TestOptions:OtherRequiredValue"] = "other",
        });

        // Act
        builder.AddAppSettings<TestOptions, TestOptionsValidator>();
        using var host = builder.Build();
        await host.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        // доменный вариант: зависимость объявляется как T, а не как IOptions<T>
        var settings = host.Services.GetRequiredService<TestOptions>();
        Assert.Equal("value", settings.RequiredValue);

        await host.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task AddAppSettings_ShouldRegisterSameInstanceAsOptionsValue()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestOptions:RequiredValue"] = "value",
            ["TestOptions:OtherRequiredValue"] = "other",
        });

        // Act
        builder.AddAppSettings<TestOptions, TestOptionsValidator>();
        using var host = builder.Build();
        await host.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(
            host.Services.GetRequiredService<IOptions<TestOptions>>().Value,
            host.Services.GetRequiredService<TestOptions>());

        await host.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task AddAppSettings_ShouldThrowOptionsValidationException_WhenConfigurationIsInvalid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestOptions:RequiredValue"] = string.Empty,
            ["TestOptions:OtherRequiredValue"] = "other",
        });

        // Act
        builder.AddAppSettings<TestOptions, TestOptionsValidator>();
        using var host = builder.Build();

        // Assert
        await Assert.ThrowsAsync<OptionsValidationException>(
            () => host.StartAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public void AddAppSettings_ShouldReturnSameBuilder()
    {
        // Arrange
        var builder = CreateBuilderWith([]);

        // Act
        var result = builder.AddAppSettings<TestOptions, TestOptionsValidator>();

        // Assert
        Assert.Same(builder, result);
    }

    private static HostApplicationBuilder CreateBuilderWith(Dictionary<string, string?> configuration)
    {
        var builder = Host.CreateEmptyApplicationBuilder(null);
        builder.Configuration.AddInMemoryCollection(configuration);

        return builder;
    }

    // имя типа задаёт имя секции конфигурации: "TestOptions"
    private sealed record TestOptions
    {
        public string RequiredValue { get; init; } = string.Empty;
        public string OtherRequiredValue { get; init; } = string.Empty;
    }

    private sealed class TestOptionsValidator : IValidateOptions<TestOptions>
    {
        public ValidateOptionsResult Validate(string? name, TestOptions options)
        {
            var failures = new List<string>();

            if (string.IsNullOrWhiteSpace(options.RequiredValue))
                failures.Add($"Не задано значение {nameof(TestOptions)}:{nameof(TestOptions.RequiredValue)}");

            if (string.IsNullOrWhiteSpace(options.OtherRequiredValue))
                failures.Add($"Не задано значение {nameof(TestOptions)}:{nameof(TestOptions.OtherRequiredValue)}");

            return failures.Count > 0
                ? ValidateOptionsResult.Fail(failures)
                : ValidateOptionsResult.Success;
        }
    }
}
