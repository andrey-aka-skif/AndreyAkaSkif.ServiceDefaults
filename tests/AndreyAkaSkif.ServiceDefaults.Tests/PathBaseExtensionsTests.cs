using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class PathBaseExtensionsTests
{
    [Fact]
    public void UseConfiguredPathBase_ShouldNotThrow_WhenConfigurationIsValid()
    {
        // Arrange
        using var app = CreateAppWith(new Dictionary<string, string?>
        {
            ["PathBaseAppSettings:Path"] = "/api",
        });

        // Act & Assert
        Assert.Same(app, app.UseConfiguredPathBase());
    }

    [Fact]
    public void UseConfiguredPathBase_ShouldNotThrow_WhenSectionIsMissing()
    {
        // Arrange
        // секции нет — базовый путь пустой, это допустимо
        using var app = CreateAppWith([]);

        // Act & Assert
        Assert.Same(app, app.UseConfiguredPathBase());
    }

    [Fact]
    public void UseConfiguredPathBase_ShouldThrowArgumentException_WhenPathIsInvalid()
    {
        // Arrange
        using var app = CreateAppWith(new Dictionary<string, string?>
        {
            ["PathBaseAppSettings:Path"] = "api",
        });

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => app.UseConfiguredPathBase());
        Assert.Equal("Path", exception.ParamName);
    }

    private static WebApplication CreateAppWith(Dictionary<string, string?> configuration)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.Configuration.AddInMemoryCollection(configuration);

        return builder.Build();
    }
}
