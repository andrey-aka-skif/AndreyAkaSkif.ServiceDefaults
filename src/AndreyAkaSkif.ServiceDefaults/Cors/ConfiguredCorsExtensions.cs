using AndreyAkaSkif.ServiceDefaults.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
    /// </remarks>
    public static IHostApplicationBuilder AddConfiguredCorsPolicy(this IHostApplicationBuilder builder)
    {
        var сorsPolicy = builder.Configuration.CreateValidated<CorsPolicy>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                сorsPolicy.Name,
                policy => policy
                    .WithOrigins(сorsPolicy.Origins)
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
    /// Требует секцию конфигурации "CorsPolicy"
    /// </remarks>
    public static WebApplication UseConfiguredCorsPolicy(this WebApplication app)
    {
        var сorsPolicy = app.Configuration.CreateValidated<CorsPolicy>();
        app.UseCors(сorsPolicy.Name);

        return app;
    }
}
