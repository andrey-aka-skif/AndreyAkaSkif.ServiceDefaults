using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class OpenApiAppSettingsValidatorTests
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
    public void Validate_ShouldSucceed_WhenDescriptionIsMissing()
    {
        // Arrange
        // описание документа необязательно
        var settings = CreateValid() with { Description = null };

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
        Assert.Contains(nameof(OpenApiAppSettings.Title), result.FailureMessage);
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("1.0.0-preview.1")]
    [InlineData("2026-08-30")]
    [InlineData("v1")]
    public void Validate_ShouldSucceed_WhenVersionIsArbitraryString(string version)
    {
        // Arrange
        // info.version по спецификации OpenAPI — произвольная строка,
        // на числовые части она не разбирается
        var settings = CreateValid() with { Version = version };

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldFail_WhenVersionIsBlank(string? version)
    {
        // Arrange
        var settings = CreateValid() with { Version = version! };

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(nameof(OpenApiAppSettings.Version), result.FailureMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldFail_WhenDocumentNameIsBlank(string? documentName)
    {
        // Arrange
        var settings = CreateValid() with { DocumentName = documentName! };

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(nameof(OpenApiAppSettings.DocumentName), result.FailureMessage);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDocumentNameContainsSlash()
    {
        // Arrange
        // имя документа — сегмент адреса спецификации
        var settings = CreateValid() with { DocumentName = "api/v1" };

        // Act
        var result = Validate(settings);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(nameof(OpenApiAppSettings.DocumentName), result.FailureMessage);
    }

    [Fact]
    public void Validate_ShouldReportAllFailures_WhenSettingsAreEmpty()
    {
        // Arrange
        // умолчания заданы инициализаторами, поэтому пустым остаётся только заголовок
        var settings = new OpenApiAppSettings();

        // Act
        var result = Validate(settings);

        // Assert
        Assert.Collection(
            result.Failures!,
            failure => Assert.Contains(nameof(OpenApiAppSettings.Title), failure));
    }

    private static OpenApiAppSettings CreateValid()
        => new()
        {
            Title = "Title",
            Description = "Description",
            DocumentName = "v1",
            Version = "1.0",
            Servers = ["/"],
        };

    private static ValidateOptionsResult Validate(OpenApiAppSettings settings)
        => new OpenApiAppSettingsValidator().Validate(Options.DefaultName, settings);
}
