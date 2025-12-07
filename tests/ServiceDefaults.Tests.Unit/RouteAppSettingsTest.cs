using AndreyAkaSkif.ServiceDefaults.Routing;
using System;

namespace AndreyAkaSkif.ServiceDefaults.Tests.Unit;

public class RouteAppSettingsTests
{
    [Theory]
    [InlineData("")]
    [InlineData("/")]
    public void Validate_ShouldNotThrow_WhenPathBaseIsEmptyStringOrSlash(string pathBase)
    {
        // Arrange
        var settings = new RouteAppSettings { PathBase = pathBase };

        // Act & Assert
        settings.Validate();
    }

    [Theory]
    [InlineData("/api")]
    [InlineData("/api/v1")]
    [InlineData("/my-app")]
    [InlineData("/test/route")]
    [InlineData("/products/v2/categories")]
    public void Validate_ShouldNotThrow_WhenPathBaseIsValid(string pathBase)
    {
        // Arrange
        var settings = new RouteAppSettings { PathBase = pathBase };

        // Act & Assert
        settings.Validate();
    }

    [Theory]
    [InlineData("/api?query=value")]
    [InlineData("/api#fragment")]
    [InlineData("/api?query=1#frag")]
    [InlineData("http://example.com")]
    [InlineData("ftp://server.com")]
    [InlineData("//relative")]
    [InlineData("api?query=1")]
    [InlineData("http://localhost")]
    [InlineData("without-leading-slash")]
    public void Validate_ShouldThrowArgumentException_WhenPathBaseContainsQueryOrFragment(string pathBase)
    {
        // Arrange
        var settings = new RouteAppSettings { PathBase = pathBase };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => settings.Validate());
        Assert.Equal("PathBase", exception.ParamName);
    }

    [Theory]
    [InlineData("invalid path")]
    [InlineData(" ")]
    [InlineData("/api/ path")]
    [InlineData("/path<test")]
    [InlineData("/path>test")]
    [InlineData("/path\"test")]
    [InlineData("/path\\test")]
    [InlineData("/api[")]
    [InlineData("/api]")]
    [InlineData("/api(")]
    [InlineData("/api)")]
    [InlineData("^/api")]
    [InlineData("/api`")]
    [InlineData("/api|")]
    [InlineData("/api:")]
    [InlineData("/api*")]
    [InlineData("/api\"")]
    [InlineData("/api\'")]
    [InlineData("/api%")]
    [InlineData("/api!")]
    [InlineData("/api@")]
    public void Validate_ShouldThrowArgumentException_WhenPathBaseIsInvalidUri(string pathBase)
    {
        // Arrange
        var settings = new RouteAppSettings { PathBase = pathBase };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => settings.Validate());
        Assert.Equal("PathBase", exception.ParamName);
    }

    [Fact]
    public void Validate_ShouldThrowArgumentException_WhenPathBaseIsNull()
    {
        // Arrange
        var settings = new RouteAppSettings { PathBase = null };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => settings.Validate());
        Assert.Equal("PathBase", exception.ParamName);
    }

    [Fact]
    public void PathBase_ShouldHaveDefaultValue_EmptyString()
    {
        // Arrange & Act
        var settings = new RouteAppSettings();

        // Assert
        Assert.Equal(string.Empty, settings.PathBase);
    }
}
