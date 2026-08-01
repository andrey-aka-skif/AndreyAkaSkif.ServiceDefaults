using AndreyAkaSkif.ServiceDefaults.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ISerilogLogger = Serilog.ILogger;
using SerilogLoggerFactory = Serilog.Extensions.Logging.SerilogLoggerFactory;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class LoggingConfigureExtensionsTests
{
    private static readonly Dictionary<string, string?> ValidConfiguration = new()
    {
        ["Serilog:MinimumLevel:Default"] = "Information",
    };

    [Fact]
    public void AddConfiguredLoggingViaSerilog_ShouldReplaceLoggerFactory_WhenExclusive()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration);

        // Act
        builder.AddConfiguredLoggingViaSerilog();
        using var app = builder.Build();

        // Assert
        // монопольная установка подменяет фабрику целиком, поэтому провайдеры
        // по умолчанию не участвуют в выводе и записи не задваиваются
        Assert.IsType<SerilogLoggerFactory>(app.Services.GetRequiredService<ILoggerFactory>());
    }

    [Fact]
    public void AddConfiguredLoggingViaSerilog_ShouldRegisterSerilogLogger_WhenExclusive()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration);

        // Act
        builder.AddConfiguredLoggingViaSerilog();
        using var app = builder.Build();

        // Assert
        Assert.NotNull(app.Services.GetService<ISerilogLogger>());
    }

    [Fact]
    public void AddConfiguredLoggingViaSerilog_ShouldKeepDefaultLoggerFactory_WhenNotExclusive()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration);

        // Act
        builder.AddConfiguredLoggingViaSerilog(exclusive: false);
        using var app = builder.Build();

        // Assert
        // Serilog добавлен как ещё один провайдер к штатной фабрике
        Assert.IsNotType<SerilogLoggerFactory>(app.Services.GetRequiredService<ILoggerFactory>());
    }

    [Fact]
    public void AddConfiguredLoggingViaSerilog_ShouldReturnSameBuilder_WhenCalled()
    {
        // Arrange
        var builder = CreateBuilderWith(ValidConfiguration);

        // Act & Assert
        Assert.Same(builder, builder.AddConfiguredLoggingViaSerilog());
    }

    private static WebApplicationBuilder CreateBuilderWith(Dictionary<string, string?> configuration)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.Configuration.AddInMemoryCollection(configuration);

        return builder;
    }
}
