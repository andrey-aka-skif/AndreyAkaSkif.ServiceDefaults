using Microsoft.AspNetCore.Routing;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class EnumRouteConstraintTests
{
    [Theory]
    [InlineData("CurrentA")]
    // сопоставление регистронезависимо
    [InlineData("currenta")]
    [InlineData("CURRENTA")]
    // числовые значения в диапазоне перечисления допустимы
    [InlineData("0")]
    [InlineData("2")]
    public void Match_ShouldReturnTrue_WhenValueBelongsToEnum(string value)
    {
        // Arrange
        var constraint = new EnumRouteConstraint<TestChannel>();
        var values = new RouteValueDictionary { ["channel"] = value };

        // Act
        var matched = Match(constraint, values);

        // Assert
        Assert.True(matched);
    }

    [Theory]
    [InlineData("unknown")]
    // Enum.TryParse разобрал бы это в (TestChannel)999 — отсекается через Enum.IsDefined
    [InlineData("999")]
    [InlineData("-1")]
    [InlineData("")]
    public void Match_ShouldReturnFalse_WhenValueIsOutsideEnum(string value)
    {
        // Arrange
        var constraint = new EnumRouteConstraint<TestChannel>();
        var values = new RouteValueDictionary { ["channel"] = value };

        // Act
        var matched = Match(constraint, values);

        // Assert
        Assert.False(matched);
    }

    [Fact]
    public void Match_ShouldReturnFalse_WhenRouteKeyIsMissing()
    {
        // Arrange
        var constraint = new EnumRouteConstraint<TestChannel>();
        var values = new RouteValueDictionary();

        // Act
        var matched = Match(constraint, values);

        // Assert
        Assert.False(matched);
    }

    [Fact]
    public void Match_ShouldReturnFalse_WhenValueIsNull()
    {
        // Arrange
        var constraint = new EnumRouteConstraint<TestChannel>();
        var values = new RouteValueDictionary { ["channel"] = null };

        // Act
        var matched = Match(constraint, values);

        // Assert
        Assert.False(matched);
    }

    [Theory]
    [InlineData("currenta", "CurrentA")]
    [InlineData("CURRENTA", "CurrentA")]
    [InlineData("2", "Torque")]
    public void Match_ShouldNormalizeValue_WhenRequestIsIncoming(string value, string expected)
    {
        // Arrange
        // привязка аргументов в минимальных API регистрозависима, поэтому значение
        // приводится к каноническому имени — иначе маршрут совпал бы, а привязка упала
        var constraint = new EnumRouteConstraint<TestChannel>();
        var values = new RouteValueDictionary { ["channel"] = value };

        // Act
        Match(constraint, values);

        // Assert
        Assert.Equal(expected, values["channel"]);
    }

    [Fact]
    public void Match_ShouldNotChangeValue_WhenDirectionIsUrlGeneration()
    {
        // Arrange
        // при генерации URL значение принадлежит вызывающему коду
        var constraint = new EnumRouteConstraint<TestChannel>();
        var values = new RouteValueDictionary { ["channel"] = "currenta" };

        // Act
        var matched = Match(constraint, values, RouteDirection.UrlGeneration);

        // Assert
        Assert.True(matched);
        Assert.Equal("currenta", values["channel"]);
    }

    [Fact]
    public void Match_ShouldAcceptEnumInstance_WhenValueIsNotString()
    {
        // Arrange
        // при генерации URL в словарь попадает само значение перечисления, а не строка
        var constraint = new EnumRouteConstraint<TestChannel>();
        var values = new RouteValueDictionary { ["channel"] = TestChannel.Torque };

        // Act
        var matched = Match(constraint, values, RouteDirection.UrlGeneration);

        // Assert
        Assert.True(matched);
    }

    private static bool Match(
        EnumRouteConstraint<TestChannel> constraint,
        RouteValueDictionary values,
        RouteDirection direction = RouteDirection.IncomingRequest)
        => constraint.Match(
            httpContext: null,
            route: null,
            routeKey: "channel",
            values: values,
            routeDirection: direction);

    private enum TestChannel
    {
        CurrentA,
        CurrentB,
        Torque,
    }
}
