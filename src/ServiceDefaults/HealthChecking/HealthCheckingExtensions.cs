using Microsoft.AspNetCore.Builder;

namespace AndreyAkaSkif.ServiceDefaults.HealthChecking;

/// <summary>
/// Методы расширения для добавления HealthCheck конечных точек
/// </summary>
public static class HealthCheckingExtensions
{
    /// <summary>
    /// Добавить HealthCheck middleware
    /// </summary>
    public static WebApplication MapHealthChecksEndpoint(this WebApplication app)
    {
        app.MapHealthChecks(HealthCheckDefaults.Endpoint);

        return app;
    }
}
