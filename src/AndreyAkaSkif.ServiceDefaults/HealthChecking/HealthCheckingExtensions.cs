using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.HealthChecking;

/// <summary>
/// Методы расширения для добавления HealthCheck конечных точек
/// </summary>
public static class HealthCheckingExtensions
{
    /// <summary>
    /// Добавить сервисы, необходимые для конечной точки проверки жизнеспособности приложения
    /// </summary>
    /// <remarks>
    /// <para>
    /// Метод регистрирует минимальную инфраструктуру ASP.NET Core Health Checks, достаточную
    /// для использования Docker Compose, Kubernetes или другого оркестратора.
    /// Саму конечную точку <c>/health</c> добавляет <see cref="MapHealthCheckEndpoint"/>
    /// </para>
    /// <para>
    /// Метод не предназначен для регистрации пользовательских проверок.
    /// Для добавления проверок БД, Redis и других зависимостей используйте стандартный API
    /// <c>builder.Services.AddHealthChecks()</c>
    /// </para>
    /// </remarks>
    /// <param name="builder">Построитель приложения</param>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    public static IHostApplicationBuilder AddHealthCheckEndpoint(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();

        return builder;
    }

    /// <summary>
    /// Добавить конечную точку проверки жизнеспособности приложения по адресу <c>/health</c>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Конечная точка позволяет оркестратору определить, что приложение запустилось и принимает
    /// HTTP-запросы. Она не исполняет зарегистрированные проверки и отвечает <c>200 Healthy</c>
    /// самим фактом ответа приложения.
    /// </para>
    /// <para>
    /// Адрес конечной точки задан константой <see cref="HealthCheckDefaults.Endpoint"/>
    /// и не конфигурируется: то же значение использует фильтр документации Swagger
    /// из пакета <c>AndreyAkaSkif.ServiceDefaults.Swagger</c>
    /// </para>
    /// <para>
    /// Проверки, добавленные через стандартный API <c>builder.Services.AddHealthChecks()</c>,
    /// сознательно не попадают в эту конечную точку: недоступность БД или другой зависимости
    /// не является поводом для перезапуска работающего приложения.
    /// Для таких проверок регистрируйте отдельную конечную точку вызовом <c>app.MapHealthChecks()</c>
    /// </para>
    /// </remarks>
    /// <param name="app">Экземпляр веб-приложения</param>
    /// <returns>Тот же экземпляр <paramref name="app"/> для поддержки цепочки вызовов</returns>
    public static WebApplication MapHealthCheckEndpoint(this WebApplication app)
    {
        app.MapHealthChecks(
            HealthCheckDefaults.Endpoint,
            new HealthCheckOptions { Predicate = _ => false });

        return app;
    }
}
