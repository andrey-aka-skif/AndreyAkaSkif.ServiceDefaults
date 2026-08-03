namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class PathBaseAppSettingsTests
{
    [Theory]
    [InlineData("")]
    [InlineData("/")]
    public void Validate_ShouldNotThrow_WhenPathIsEmptyStringOrSlash(string path)
    {
        // Arrange
        var settings = new PathBaseAppSettings { Path = path };

        // Act & Assert
        settings.Validate();
    }

    [Theory]
    [InlineData("/api")]
    [InlineData("/api/v1")]
    [InlineData("/my-app")]
    [InlineData("/test/route")]
    [InlineData("/products/v2/categories")]
    // хвостовой слеш безопасен: UsePathBase делает TrimEnd('/')
    [InlineData("/api/")]
    // остальные unreserved-символы RFC 3986
    [InlineData("/api/v1.0")]
    [InlineData("/my_app")]
    [InlineData("/api~x")]
    public void Validate_ShouldNotThrow_WhenPathIsValid(string path)
    {
        // Arrange
        var settings = new PathBaseAppSettings { Path = path };

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
    public void Validate_ShouldThrowArgumentException_WhenPathContainsQueryOrFragment(string path)
    {
        // Arrange
        var settings = new PathBaseAppSettings { Path = path };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => settings.Validate());
        Assert.Equal("Path", exception.ParamName);
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
    public void Validate_ShouldThrowArgumentException_WhenPathIsInvalidUri(string path)
    {
        // Arrange
        var settings = new PathBaseAppSettings { Path = path };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => settings.Validate());
        Assert.Equal("Path", exception.ParamName);
    }

    [Theory]
    // символы, которые пропускал прежний чёрный список
    [InlineData("/api;v=1")]
    [InlineData("/api&x=1")]
    [InlineData("/api=1")]
    [InlineData("/api+b")]
    [InlineData("/api,b")]
    [InlineData("/api$")]
    [InlineData("/api{x}")]
    [InlineData("/api\n")]
    [InlineData("/api\t")]
    [InlineData("/api\0")]
    public void Validate_ShouldThrowArgumentException_WhenPathContainsCharOutsideAllowedAlphabet(string path)
    {
        // Arrange
        var settings = new PathBaseAppSettings { Path = path };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => settings.Validate());
        Assert.Equal("Path", exception.ParamName);
    }

    [Theory]
    // \w и \d в .NET учитывают Unicode — класс символов задан явно, чтобы это не прошло
    [InlineData("/апи")]
    [InlineData("/straße")]
    // percent-encoding бесполезен: сравнение идёт с уже раскодированным Request.Path
    [InlineData("/%D0%B0%D0%BF%D0%B8")]
    public void Validate_ShouldThrowArgumentException_WhenPathIsNotAscii(string path)
    {
        // Arrange
        var settings = new PathBaseAppSettings { Path = path };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => settings.Validate());
        Assert.Equal("Path", exception.ParamName);
    }

    [Fact]
    public void Validate_ShouldDescribeExpectedFormat_WhenPathIsInvalid()
    {
        // Arrange
        var settings = new PathBaseAppSettings { Path = "/апи" };

        // Act & Assert
        // сообщение самодостаточно: конфигурационная ошибка часто видна только в логах
        var exception = Assert.Throws<ArgumentException>(() => settings.Validate());
        Assert.Contains("/апи", exception.Message, StringComparison.Ordinal);
        Assert.Contains("A-Za-z0-9", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Validate_ShouldThrowArgumentException_WhenPathIsNull()
    {
        // Arrange
        // намеренная передача null в non-nullable свойство — проверяем защиту Validate
        var settings = new PathBaseAppSettings { Path = null! };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => settings.Validate());
        Assert.Equal("Path", exception.ParamName);
    }

    [Fact]
    public void Path_ShouldHaveDefaultValue_EmptyString()
    {
        // Arrange & Act
        var settings = new PathBaseAppSettings();

        // Assert
        Assert.Equal(string.Empty, settings.Path);
    }
}
