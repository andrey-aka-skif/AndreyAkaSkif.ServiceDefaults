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
    public static WebApplication UsePermissiveCorsPolicy(this WebApplication app)
    {
        app.UseCors(ALLOW_ALL_POLICY_NAME);

        return app;
    }
}
