using AndreyAkaSkif.ServiceDefaults.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// переезд на конвейер параметров выполняется отдельным шагом
#pragma warning disable CS0618

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
    /// Валидированные настройки регистрируются в DI-контейнере,
    /// откуда их берёт <see cref="UseConfiguredCorsPolicy"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если секция "CorsPolicy" отсутствует в конфигурации или содержит некорректные данные.
    /// </exception>
    public static IHostApplicationBuilder AddConfiguredCorsPolicy(this IHostApplicationBuilder builder)
    {
        var corsPolicy = builder.Configuration.CreateValidated<CorsPolicy>();

        builder.AddServiceArg(corsPolicy);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                corsPolicy.Name,
                policy => policy
                    .WithOrigins(corsPolicy.Origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
        });

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
        var corsPolicy = app.Services.GetRequiredService<CorsPolicy>();
        app.UseCors(corsPolicy.Name);

        return app;
    }
}
