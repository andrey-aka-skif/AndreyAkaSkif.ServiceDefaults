using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class RouteConstraintExtensionsTests
{
    [Fact]
    public void AddEnumRouteConstraint_ShouldUseCamelCasedTypeName_WhenNameIsNotSpecified()
    {
        // Arrange
        var builder = Host.CreateEmptyApplicationBuilder(null);

        // Act
        builder.AddEnumRouteConstraint<TestChannel>();

        // Assert
        var constraintMap = ResolveConstraintMap(builder);
        Assert.Equal(typeof(EnumRouteConstraint<TestChannel>), constraintMap["testChannel"]);
    }

    [Fact]
    public void AddEnumRouteConstraint_ShouldUseExplicitName_WhenNameIsSpecified()
    {
        // Arrange
        var builder = Host.CreateEmptyApplicationBuilder(null);

        // Act
        builder.AddEnumRouteConstraint<TestChannel>("channel");

        // Assert
        var constraintMap = ResolveConstraintMap(builder);
        Assert.Equal(typeof(EnumRouteConstraint<TestChannel>), constraintMap["channel"]);
        Assert.False(constraintMap.ContainsKey("testChannel"));
    }

    [Fact]
    public void AddRouteConstraint_ShouldNotThrow_WhenSameTypeIsRegisteredTwice()
    {
        // Arrange
        var builder = Host.CreateEmptyApplicationBuilder(null);

        // Act
        builder.AddEnumRouteConstraint<TestChannel>("channel");
        builder.AddEnumRouteConstraint<TestChannel>("channel");

        // Assert
        var constraintMap = ResolveConstraintMap(builder);
        Assert.Equal(typeof(EnumRouteConstraint<TestChannel>), constraintMap["channel"]);
    }

    [Fact]
    public void AddRouteConstraint_ShouldThrowInvalidOperationException_WhenNameIsTakenByAnotherType()
    {
        // Arrange
        var builder = Host.CreateEmptyApplicationBuilder(null);
        builder.AddEnumRouteConstraint<TestChannel>("channel");
        builder.AddEnumRouteConstraint<TestUnit>("channel");

        // Act & Assert
        // делегат Configure выполняется при разрешении RouteOptions, а не при регистрации
        var exception = Assert.Throws<InvalidOperationException>(() => ResolveConstraintMap(builder));
        Assert.Contains("channel", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AddRouteConstraint_ShouldNotReplaceBuiltInConstraints()
    {
        // Arrange
        var builder = Host.CreateEmptyApplicationBuilder(null);

        // Act
        builder.AddEnumRouteConstraint<TestChannel>();

        // Assert
        var constraintMap = ResolveConstraintMap(builder);
        Assert.Equal(typeof(IntRouteConstraint), constraintMap["int"]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void AddEnumRouteConstraint_ShouldThrowArgumentException_WhenNameIsBlank(string? name)
    {
        // Arrange
        var builder = Host.CreateEmptyApplicationBuilder(null);

        // Act & Assert
        // пустое имя отвергается сразу, а не при разрешении RouteOptions:
        // null означает имя по умолчанию и до этой проверки не доходит
        var exception = Assert.Throws<ArgumentException>(
            () => builder.AddRouteConstraint<EnumRouteConstraint<TestChannel>>(name!));
        Assert.Equal("name", exception.ParamName);
    }

    private static IDictionary<string, Type> ResolveConstraintMap(HostApplicationBuilder builder)
    {
        using var host = builder.Build();

        return host.Services.GetRequiredService<IOptions<RouteOptions>>().Value.ConstraintMap;
    }

    private enum TestChannel
    {
        CurrentA,
        Torque,
    }

    private enum TestUnit
    {
        Volt,
        Ampere,
    }
}
