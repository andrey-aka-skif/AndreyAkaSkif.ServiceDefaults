using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class SwaggerAppSettingsValidatorTests
{
    [Fact]
    public void Validate_ShouldSucceed_WhenSectionIsMissing()
    {
        // Arrange
        // ни один ключ секции не обязателен: адрес спецификации имеет умолчание
        var settings = new SwaggerAppSettings();

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldFail_WhenUrlIsBlank(string? url)
    {
        // Arrange
        var settings = new SwaggerAppSettings { Url = url! };

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(nameof(SwaggerAppSettings.Url), result.FailureMessage);
    }

    [Fact]
    public void DisplayName_ShouldFallBackToUrl_WhenNameIsMissing()
    {
        // Arrange
        var settings = new SwaggerAppSettings { Url = "/openapi/internal.json" };

        // Act & Assert
        Assert.Equal("/openapi/internal.json", settings.DisplayName);
    }

    [Fact]
    public void DisplayName_ShouldBeName_WhenNameIsSet()
    {
        // Arrange
        var settings = new SwaggerAppSettings { Name = "Demo API" };

        // Act & Assert
        Assert.Equal("Demo API", settings.DisplayName);
    }

    private static ValidateOptionsResult Validate(SwaggerAppSettings settings)
        => new SwaggerAppSettingsValidator().Validate(Options.DefaultName, settings);
}
