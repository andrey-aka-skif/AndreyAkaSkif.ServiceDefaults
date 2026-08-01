using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AndreyAkaSkif.ServiceDefaults.Serilog;

/// <summary>
/// Предоставляет методы расширения для настройки логирования Serilog в приложении.
/// </summary>
public static class LoggingConfigureExtensions
{
    /// <summary>
    /// Добавляет и настраивает Serilog как систему логирования для приложения
    /// </summary>
    /// <remarks>
    /// <para>
    /// В файле конфигурации приложения "appsettings.json" требуется секция "Serilog"
    /// </para>
    /// <para>
    /// При <paramref name="exclusive"/> = <see langword="true"/> (по умолчанию) Serilog
    /// устанавливается монопольно: подменяется <c>ILoggerFactory</c>, поэтому провайдеры
    /// по умолчанию, включая консольный, перестают писать и каждая запись попадает
    /// в вывод один раз. Вызывать <c>builder.Logging.ClearProviders()</c> не требуется.
    /// Дополнительно в DI-контейнере регистрируются <c>Serilog.ILogger</c>
    /// и <c>IDiagnosticContext</c>.
    /// </para>
    /// <para>
    /// Следует обратить внимание, что при монопольной установке молча перестают
    /// работать и провайдеры, добавленные самим приложением через <c>builder.Logging</c>:
    /// заменяется фабрика целиком, а не список провайдеров. Если Serilog нужен рядом
    /// с другими провайдерами — <paramref name="exclusive"/> = <see langword="false"/>;
    /// в этом случае Serilog добавляется к уже настроенным провайдерам, и очистку
    /// лишних выполняет приложение.
    /// </para>
    /// <para>
    /// Время жизни логгера передаётся хосту: логгер закрывается при завершении
    /// приложения, поэтому буферизующие стоки успевают сбросить накопленные записи.
    /// </para>
    /// </remarks>
    /// <param name="builder">
    /// Экземпляр <see cref="IHostApplicationBuilder"/>, для которого настраивается логирование.
    /// </param>
    /// <param name="exclusive">
    /// <see langword="true"/> — установить Serilog монопольно, вытеснив остальные провайдеры;
    /// <see langword="false"/> — добавить Serilog к уже настроенным провайдерам.
    /// </param>
    /// <returns>
    /// Тот же экземпляр <see cref="IHostApplicationBuilder"/> для поддержки цепочки вызовов.
    /// </returns>
    public static IHostApplicationBuilder AddConfiguredLoggingViaSerilog(
        this IHostApplicationBuilder builder,
        bool exclusive = true)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        if (exclusive)
            builder.Services.AddSerilog(logger, dispose: true);
        else
            builder.Logging.AddSerilog(logger, dispose: true);

        return builder;
    }
}
