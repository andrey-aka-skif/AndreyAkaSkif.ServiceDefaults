using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.HealthChecking;

/// <summary>
/// Методы расширения для добавления HealthCheck конечных точек
/// </summary>
public static class HealthCheckingExtensions
{
    /// <summary>
    /// Добавить HealthCheck сервисы
    /// </summary>
    /// <remarks>
    /// Метод является унифицированной оберткой над вызовом <c>builder.Services.AddHealthChecks()</c>
    /// </remarks>
    public static IHostApplicationBuilder AddHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();

        return builder;
    }

    /// <summary>
    /// Добавить HealthCheck middleware
    /// </summary>
    public static WebApplication MapHealthChecksEndpoint(this WebApplication app)
    {
        app.MapHealthChecks(HealthCheckDefaults.Endpoint);

        return app;
    }
}
