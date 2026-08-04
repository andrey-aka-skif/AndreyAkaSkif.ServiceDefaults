using AndreyAkaSkif.ServiceDefaults.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using CorsOptions = Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions;

namespace AndreyAkaSkif.ServiceDefaults.Cors;

/// <summary>
/// Методы расширения для регистрации конфигурируемых политик CORS в DI-контейнере
/// </summary>
public static class ConfiguredCorsExtensions
{
    /// <summary>
    /// Добавить политику CORS, настроенную через конфигурацию
    /// </summary>
    /// <remarks>
    /// <para>
    /// Требует секцию конфигурации "CorsPolicy" следующего вида:
    /// <code>
    /// "CorsPolicy": {
    ///   "Name": "PolicyName",
    ///   "Origins": [
    ///     "http://localhost:5001",
    ///     "http://example.com"
    ///   ]
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Настройки регистрируются в конвейере параметров, откуда их берут и сама политика,
    /// и <see cref="UseConfiguredCorsPolicy"/>. Отсутствующая или некорректная секция
    /// роняет приложение при старте хоста, до первого запроса.
    /// </para>
    /// </remarks>
    public static IHostApplicationBuilder AddConfiguredCorsPolicy(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AddValidatedOptions<CorsPolicy, CorsPolicyValidator>();

        builder.Services.AddCors();

        // политика собирается лениво: на момент вызова настройки ещё не привязаны
        builder.Services
            .AddOptions<CorsOptions>()
            .Configure<IOptions<CorsPolicy>>((corsOptions, corsPolicy) =>
                corsOptions.AddPolicy(
                    corsPolicy.Value.Name,
                    policy => policy
                        .WithOrigins(corsPolicy.Value.Origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()));

        return builder;
    }

    /// <summary>
    /// Использовать политику CORS, настроенную через конфигурацию
    /// </summary>
    /// <remarks>
    /// Требует предварительного вызова <see cref="AddConfiguredCorsPolicy"/>:
    /// настройки берутся из DI-контейнера, конфигурация повторно не читается.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается, если <see cref="AddConfiguredCorsPolicy"/> не был вызван.
    /// </exception>
    public static WebApplication UseConfiguredCorsPolicy(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // без парного Add* конвейер отдал бы пустые настройки и политика применилась
        // бы молча, поэтому наличие правил валидации проверяется явно
        if (app.Services.GetService<IValidateOptions<CorsPolicy>>() is null)
            throw new InvalidOperationException(
                $"Требуется вызов {nameof(AddConfiguredCorsPolicy)}()");

        var corsPolicy = app.Services.GetRequiredService<IOptions<CorsPolicy>>().Value;
        app.UseCors(corsPolicy.Name);

        return app;
    }
}
