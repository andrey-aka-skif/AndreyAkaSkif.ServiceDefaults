using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class SwaggerAppSettingsValidatorTests
{
    [Fact]
    public void Validate_ShouldSucceed_WhenSettingsAreValid()
    {
        // Arrange
        var settings = CreateValid();

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_ShouldSucceed_WhenServersAreEmpty()
    {
        // Arrange
        // список серверов необязателен: пустой заполняется в PostConfigure
        var settings = CreateValid() with { Servers = [] };

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldFail_WhenTitleIsBlank(string? title)
    {
        // Arrange
        var settings = CreateValid() with { Title = title! };

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(nameof(SwaggerAppSettings.Title), result.FailureMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldFail_WhenDescriptionIsBlank(string? description)
    {
        // Arrange
        var settings = CreateValid() with { Description = description! };

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(nameof(SwaggerAppSettings.Description), result.FailureMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("version")]
    [InlineData("1")]
    public void Validate_ShouldFail_WhenApiVersionIsNotAVersion(string? apiVersion)
    {
        // Arrange
        var settings = CreateValid() with { ApiVersion = apiVersion! };

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(nameof(SwaggerAppSettings.ApiVersion), result.FailureMessage);
    }

    [Fact]
    public void Validate_ShouldReportAllFailures_WhenSettingsAreEmpty()
    {
        // Arrange
        var settings = new SwaggerAppSettings();

        // Act
        var result = Validate(settings);

        // Assert
        Assert.Collection(
            result.Failures!,
            failure => Assert.Contains(nameof(SwaggerAppSettings.Title), failure),
            failure => Assert.Contains(nameof(SwaggerAppSettings.Description), failure),
            failure => Assert.Contains(nameof(SwaggerAppSettings.ApiVersion), failure));
    }

    private static SwaggerAppSettings CreateValid()
        => new()
        {
            Title = "Title",
            Description = "Description",
            ApiVersion = "1.0",
            Servers = ["/"],
        };

    private static ValidateOptionsResult Validate(SwaggerAppSettings settings)
        => new SwaggerAppSettingsValidator().Validate(Options.DefaultName, settings);
}
