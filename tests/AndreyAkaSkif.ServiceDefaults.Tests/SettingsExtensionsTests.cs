using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class SettingsExtensionsTests
{
    [Fact]
    public void AddServiceArgFromValidatedSettingsObject_ShouldThrowArgumentException_WhenConfigurationIsInvalid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestSettings:RequiredValue"] = string.Empty,
        });

        // Act & Assert
        // валидация выполняется в момент регистрации, а не при разрешении сервиса
        var exception = Assert.Throws<ArgumentException>(
            () => builder.AddServiceArgFromValidatedSettingsObject<TestSettings>());
        Assert.Equal("RequiredValue", exception.ParamName);
    }

    [Fact]
    public void AddServiceArgFromValidatedSettingsObject_ShouldRegisterBoundSettings_WhenConfigurationIsValid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestSettings:RequiredValue"] = "value",
        });

        // Act
        builder.AddServiceArgFromValidatedSettingsObject<TestSettings>();
        using var host = builder.Build();

        // Assert
        var settings = host.Services.GetRequiredService<TestSettings>();
        Assert.Equal("value", settings.RequiredValue);
    }

    [Fact]
    public void AddServiceArg_ShouldRegisterSameInstance()
    {
        // Arrange
        var builder = CreateBuilderWith([]);
        var arg = new TestSettings { RequiredValue = "value" };

        // Act
        builder.AddServiceArg(arg);
        using var host = builder.Build();

        // Assert
        Assert.Same(arg, host.Services.GetRequiredService<TestSettings>());
    }

    private static HostApplicationBuilder CreateBuilderWith(Dictionary<string, string?> configuration)
    {
        var builder = Host.CreateEmptyApplicationBuilder(null);
        builder.Configuration.AddInMemoryCollection(configuration);

        return builder;
    }

    // имя типа задаёт имя секции конфигурации: "TestSettings"
    private sealed record TestSettings : IValidatableSettingsObject
    {
        public string RequiredValue { get; init; } = string.Empty;

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(RequiredValue))
                throw new ArgumentException(
                    $"Не задано значение {nameof(TestSettings)}:{nameof(RequiredValue)}",
                    nameof(RequiredValue));
        }
    }
}
