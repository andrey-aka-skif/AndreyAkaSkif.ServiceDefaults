using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class CorsPolicyValidatorTests
{
    [Fact]
    public void Validate_ShouldSucceed_WhenSettingsAreValid()
    {
        // Arrange
        var settings = new CorsPolicy
        {
            Name = "PolicyName",
            Origins = ["http://localhost:5001"],
        };

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldFail_WhenNameIsBlank(string? name)
    {
        // Arrange
        var settings = new CorsPolicy
        {
            Name = name!,
            Origins = ["http://localhost:5001"],
        };

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(nameof(CorsPolicy.Name), result.FailureMessage);
    }

    [Fact]
    public void Validate_ShouldFail_WhenOriginsAreEmpty()
    {
        // Arrange
        var settings = new CorsPolicy { Name = "PolicyName", Origins = [] };

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(nameof(CorsPolicy.Origins), result.FailureMessage);
    }

    [Fact]
    public void Validate_ShouldReportAllFailures_WhenSettingsAreEmpty()
    {
        // Arrange
        var settings = new CorsPolicy();

        // Act
        var result = Validate(settings);

        // Assert
        Assert.Collection(
            result.Failures!,
            failure => Assert.Contains(nameof(CorsPolicy.Name), failure),
            failure => Assert.Contains(nameof(CorsPolicy.Origins), failure));
    }

    private static ValidateOptionsResult Validate(CorsPolicy settings)
        => new CorsPolicyValidator().Validate(Options.DefaultName, settings);
}
