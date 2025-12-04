using Microsoft.Extensions.Hosting;
using Serilog;

namespace AndreyAkaSkif.ServiceDefaults.Logging.Serilog;

/// <summary>
/// Предоставляет методы расширения для настройки логирования Serilog в приложении.
/// </summary>
public static class LoggingConfigureExtensions
{
    /// <summary>
    /// Настраивает Serilog как систему логирования для приложения, 
    /// используя конфигурацию из <see cref="IHostApplicationBuilder.Configuration"/>.
    /// </summary>
    /// <remarks>
    /// В файле конфигурации приложения "appsettings.json" требуется секция "Serilog"
    /// </remarks>
    /// <param name="builder">
    /// Экземпляр <see cref="IHostApplicationBuilder"/>, для которого настраивается логирование.
    /// </param>
    /// <returns>
    /// Тот же экземпляр <see cref="IHostApplicationBuilder"/> для поддержки цепочки вызовов.
    /// </returns>
    public static IHostApplicationBuilder SetupSerilogLogging(this IHostApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Logging.AddSerilog(logger);

        return builder;
    }
}
