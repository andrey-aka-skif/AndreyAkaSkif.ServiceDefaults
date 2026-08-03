using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class StringEnumJsonExtensionsTests
{
    [Fact]
    public void AddStringEnumJsonSerialization_ShouldConfigureHttpJsonOptions()
    {
        // Arrange
        var builder = WebApplication.CreateSlimBuilder();

        // Act
        builder.AddStringEnumJsonSerialization();
        using var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<HttpJsonOptions>>().Value;
        Assert.Equal("\"Torque\"", Serialize(options.SerializerOptions));
    }

    [Fact]
    public void AddStringEnumJsonSerialization_ShouldConfigureMvcJsonOptions()
    {
        // Arrange
        // генератор спецификации Swashbuckle читает именно MVC-параметры: без них тела
        // ответов содержали бы имена, а спецификация объявляла бы перечисление числовым
        var builder = WebApplication.CreateSlimBuilder();

        // Act
        builder.AddStringEnumJsonSerialization();
        using var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<MvcJsonOptions>>().Value;
        Assert.Equal("\"Torque\"", Serialize(options.JsonSerializerOptions));
    }

    [Fact]
    public void AddStringEnumJsonSerialization_ShouldAddConverterOnce_WhenCalledRepeatedly()
    {
        // Arrange
        // метод вызывается из AddEnumRouteConstraint для каждого перечисления
        var builder = WebApplication.CreateSlimBuilder();

        // Act
        builder.AddStringEnumJsonSerialization();
        builder.AddStringEnumJsonSerialization();
        builder.AddStringEnumJsonSerialization();
        using var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<HttpJsonOptions>>().Value;
        Assert.Single(options.SerializerOptions.Converters, c => c is JsonStringEnumConverter);
    }

    [Fact]
    public void AddEnumRouteConstraint_ShouldEnableStringEnumSerialization()
    {
        // Arrange
        // ограничение маршрута без строковой сериализации решало бы задачу наполовину:
        // спецификация продолжала бы объявлять перечисление целочисленным
        var builder = WebApplication.CreateSlimBuilder();

        // Act
        builder.AddEnumRouteConstraint<TestChannel>();
        using var app = builder.Build();

        // Assert
        var options = app.Services.GetRequiredService<IOptions<HttpJsonOptions>>().Value;
        Assert.Equal("\"Torque\"", Serialize(options.SerializerOptions));
    }

    [Fact]
    public void HttpJsonOptions_ShouldSerializeEnumAsNumber_WhenExtensionIsNotCalled()
    {
        // Arrange
        var builder = WebApplication.CreateSlimBuilder();

        // Act
        using var app = builder.Build();

        // Assert
        // подтверждает исходную задачу: по умолчанию перечисление уезжает числом
        var options = app.Services.GetRequiredService<IOptions<HttpJsonOptions>>().Value;
        Assert.Equal("2", Serialize(options.SerializerOptions));
    }

    private static string Serialize(JsonSerializerOptions options)
        => JsonSerializer.Serialize(TestChannel.Torque, options);

    private enum TestChannel
    {
        CurrentA,
        VoltageA,
        Torque,
    }
}
