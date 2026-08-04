using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.Cors;

/// <summary>
/// Методы расширения для регистрации разрешительных политик CORS в DI-контейнере
/// </summary>
public static class PermissiveCorsExtensions
{
    private const string ALLOW_ALL_POLICY_NAME = "AllowAll";

    /// <summary>
    /// Добавить политику CORS, разрешающую все источники
    /// </summary>
    /// <remarks>
    /// Политика предназначена для локальной разработки. В продуктовой среде список
    /// источников следует задавать явно, через
    /// <see cref="ConfiguredCorsExtensions.AddConfiguredCorsPolicy(IHostApplicationBuilder)"/>
    /// </remarks>
    /// <param name="builder">Построитель приложения</param>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
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
    /// Использовать политику CORS, разрешающую все источники
    /// </summary>
    /// <remarks>
    /// Требует предварительного вызова
    /// <see cref="AddPermissiveCorsPolicy(IHostApplicationBuilder)"/>
    /// </remarks>
    /// <param name="app">Экземпляр веб-приложения</param>
    /// <returns>Тот же экземпляр <paramref name="app"/> для поддержки цепочки вызовов</returns>
    public static WebApplication UsePermissiveCorsPolicy(this WebApplication app)
    {
        app.UseCors(ALLOW_ALL_POLICY_NAME);

        return app;
    }
}
