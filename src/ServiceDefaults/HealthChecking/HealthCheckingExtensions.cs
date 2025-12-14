using Microsoft.AspNetCore.Builder;

namespace AndreyAkaSkif.ServiceDefaults.HealthChecking;

/// <summary>
/// Методы расширения для добавления HealthCheck конечных точек
/// </summary>
public static class HealthCheckingExtensions
{
    /// <summary>
    /// Конечная точка HealthCheck
    /// </summary>
    public const string HEALTH_ENDPOINT = "/health";

    /// <summary>
    /// Добавить HealthCheck middleware
    /// </summary>
    public static WebApplication UseSimpleHealthCheck(this WebApplication app)
    {
        app.MapHealthChecks(HEALTH_ENDPOINT);

        return app;
    }
}
