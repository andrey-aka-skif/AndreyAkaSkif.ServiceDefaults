namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class PathBaseAppSettingsTests
{
    [Fact]
    public void Path_ShouldHaveDefaultValue_EmptyString()
    {
        // Arrange & Act
        var settings = new PathBaseAppSettings();

        // Assert
        Assert.Equal(string.Empty, settings.Path);
    }
}
