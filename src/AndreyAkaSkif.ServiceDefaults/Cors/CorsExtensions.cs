using AndreyAkaSkif.ServiceDefaults.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.Cors;

/// <summary>
/// Методы расширения для регистрации политик CORS в DI-контейнере.
/// </summary>
public static class CorsExtensions
{
    private const string ALLOW_ALL_POLICY_NAME = "AllowAll";

    /// <summary>
    /// Добавить политику CORS, разрешающую все источники
    /// </summary>
    public static IHostApplicationBuilder AddPermissiveCorsPolicy(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                ALLOW_ALL_POLICY_NAME,
                policy => policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
        });

        return builder;
    }

    /// <summary>
    /// Добавить политику CORS, настроенную через конфигурацию.
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
    /// <returns>
    /// Объект настроек типа <see cref="CorsPolicy"/>
    /// </returns>

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
    /// Использовать политику CORS, разрешающую все источники
    /// </summary>
    public static IApplicationBuilder UsePermissiveCorsPolicy(this WebApplication app)
    {
        app.UseCors(ALLOW_ALL_POLICY_NAME);

        return app;
    }

    /// <summary>
    /// Использовать политику CORS, настроенную через конфигурацию
    /// </summary>
    /// <remarks>
    /// Требует секцию конфигурации "CorsPolicy"
    /// </remarks>
    public static IApplicationBuilder UseConfiguredCorsPolicy(this WebApplication app)
    {
        var сorsPolicy = app.Configuration.CreateValidated<CorsPolicy>();
        app.UseCors(сorsPolicy.Name);

        return app;
    }
}
