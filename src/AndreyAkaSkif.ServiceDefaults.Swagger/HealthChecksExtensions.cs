using AndreyAkaSkif.ServiceDefaults.HealthChecking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.Swagger;

/// <summary>
/// Методы расширения для отображения HealthCheck конечных точек в Swagger
/// </summary>
public static class HealthChecksExtensions
{
    /// <summary>
    /// Добавить сервисы конечной точки проверки жизнеспособности приложения
    /// и её отображение в Swagger
    /// </summary>
    /// <remarks>
    /// Пара вызовов <see cref="HealthCheckingExtensions.AddHealthCheckEndpoint(IHostApplicationBuilder)"/>
    /// и <see cref="AddHealthCheckEndpointSwagger(IHostApplicationBuilder)"/>. Саму конечную
    /// точку по-прежнему добавляет <c>MapHealthCheckEndpoint()</c>
    /// </remarks>
    /// <param name="builder">Построитель приложения</param>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    public static IHostApplicationBuilder AddHealthCheckEndpointWithSwagger(this IHostApplicationBuilder builder)
    {
        builder.AddHealthCheckEndpoint();
        builder.AddHealthCheckEndpointSwagger();

        return builder;
    }

    /// <summary>
    /// Добавить в Swagger отображение конечной точки проверки жизнеспособности приложения
    /// </summary>
    /// <remarks>
    /// <para>
    /// В документацию добавляется конечная точка <c>/health</c>
    /// (константа <see cref="HealthCheckDefaults.Endpoint"/>) с единственным ответом <c>200 Healthy</c>
    /// </para>
    /// <para>
    /// Обратить внимание, что метод только добавляет описание конечной точки в документацию Swagger.
    /// Для функционирования конечной точки необходимо включить HealthCheck сервисы и добавить HealthCheck middleware в конвейер обработки запросов.
    /// В ином случае конечная точка будет неактивна. Соответствующий пункт Swagger UI будет возвращать ошибку 404 Not Found
    /// </para>
    /// <para>
    /// Метод регистрирует ApiExplorer, на котором строится генерация спецификации:
    /// отдельный вызов <c>AddEndpointsApiExplorer()</c> в приложении не требуется.
    /// </para>
    /// </remarks>
    /// <param name="builder">Построитель приложения</param>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    public static IHostApplicationBuilder AddHealthCheckEndpointSwagger(this IHostApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.DocumentFilter<HealthChecksDocumentFilter>();
        });

        return builder;
    }
}
