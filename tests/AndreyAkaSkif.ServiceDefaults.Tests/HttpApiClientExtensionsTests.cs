using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class HttpApiClientExtensionsTests
{
    [Fact]
    public void AddApiClient_ShouldSetBaseAddress_WhenSectionIsValid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestApiClientOptions:BaseAddress"] = "https://api.example.com/",
        });

        // Act
        builder.AddApiClient<TestApiClient, TestApiClientOptions>();
        using var host = builder.Build();

        // Assert
        var client = host.Services.GetRequiredService<TestApiClient>();
        Assert.Equal(new Uri("https://api.example.com/"), client.BaseAddress);
    }

    [Fact]
    public void AddApiClient_ShouldKeepClientsIndependent_WhenSeveralClientsRegistered()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestApiClientOptions:BaseAddress"] = "https://first.example.com/",
            ["OtherApiClientOptions:BaseAddress"] = "https://second.example.com/",
        });

        // Act
        builder.AddApiClient<TestApiClient, TestApiClientOptions>();
        builder.AddApiClient<OtherApiClient, OtherApiClientOptions>();
        using var host = builder.Build();

        // Assert
        // каждый клиент читает свою секцию: имя секции берётся из типа настроек,
        // а не из имени параметра типа
        Assert.Equal(
            new Uri("https://first.example.com/"),
            host.Services.GetRequiredService<TestApiClient>().BaseAddress);
        Assert.Equal(
            new Uri("https://second.example.com/"),
            host.Services.GetRequiredService<OtherApiClient>().BaseAddress);
    }

    [Fact]
    public void AddApiClient_ShouldReadExplicitSection_WhenSectionNameIsProvided()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestApiClientOptions:BaseAddress"] = "https://ignored.example.com/",
            ["ExternalApi:BaseAddress"] = "https://api.example.com/",
        });

        // Act
        builder.AddApiClient<TestApiClient, TestApiClientOptions>("ExternalApi");
        using var host = builder.Build();

        // Assert
        var client = host.Services.GetRequiredService<TestApiClient>();
        Assert.Equal(new Uri("https://api.example.com/"), client.BaseAddress);
    }

    [Fact]
    public void AddApiClient_ShouldThrowOptionsValidationException_WhenSectionIsMissing()
    {
        // Arrange
        var builder = CreateBuilderWith([]);

        // Act
        builder.AddApiClient<TestApiClient, TestApiClientOptions>();
        using var host = builder.Build();

        // Assert
        var exception = Assert.Throws<OptionsValidationException>(
            host.Services.GetRequiredService<TestApiClient>);
        Assert.Contains("TestApiClientOptions:BaseAddress", exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("api.example.com")]
    [InlineData("/api")]
    public void AddApiClient_ShouldThrowOptionsValidationException_WhenBaseAddressIsNotAbsolute(
        string baseAddress)
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestApiClientOptions:BaseAddress"] = baseAddress,
        });

        // Act
        builder.AddApiClient<TestApiClient, TestApiClientOptions>();
        using var host = builder.Build();

        // Assert
        Assert.Throws<OptionsValidationException>(
            host.Services.GetRequiredService<TestApiClient>);
    }

    [Fact]
    public async Task AddApiClient_ShouldFailOnHostStart_WhenBaseAddressIsInvalid()
    {
        // Arrange
        var builder = CreateBuilderWith(new Dictionary<string, string?>
        {
            ["TestApiClientOptions:BaseAddress"] = "not-a-url",
        });

        // Act
        builder.AddApiClient<TestApiClient, TestApiClientOptions>();
        using var host = builder.Build();

        // Assert
        // ValidateOnStart: приложение не поднимается, ошибка не откладывается
        // до первого обращения к внешнему API
        await Assert.ThrowsAsync<OptionsValidationException>(
            () => host.StartAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public void AddApiClient_ShouldThrowArgumentException_WhenSectionNameIsEmpty()
    {
        // Arrange
        var builder = CreateBuilderWith([]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => builder.AddApiClient<TestApiClient, TestApiClientOptions>("   "));
        Assert.Equal("sectionName", exception.ParamName);
    }

    [Fact]
    public void AddApiClient_ShouldReturnSameBuilder()
    {
        // Arrange
        var builder = CreateBuilderWith([]);

        // Act & Assert
        Assert.Same(builder, builder.AddApiClient<TestApiClient, TestApiClientOptions>());
    }

    private static HostApplicationBuilder CreateBuilderWith(Dictionary<string, string?> configuration)
    {
        var builder = Host.CreateEmptyApplicationBuilder(null);
        builder.Configuration.AddInMemoryCollection(configuration);

        return builder;
    }

    // имя типа задаёт имя секции конфигурации: "TestApiClientOptions"
    internal sealed class TestApiClientOptions : IHttpApiClientOptions
    {
        public string BaseAddress { get; set; } = string.Empty;
    }

    internal sealed class OtherApiClientOptions : IHttpApiClientOptions
    {
        public string BaseAddress { get; set; } = string.Empty;
    }

    internal sealed class TestApiClient(HttpClient httpClient)
    {
        public Uri? BaseAddress => httpClient.BaseAddress;
    }

    internal sealed class OtherApiClient(HttpClient httpClient)
    {
        public Uri? BaseAddress => httpClient.BaseAddress;
    }
}
