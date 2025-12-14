using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.Swagger;

/// <summary>
/// Методы расширения для отображения HealthCheck конечных точек в Swagger
/// </summary>
public static class HealthChecksExtensions
{
    /// <summary>
    /// Добавить в Swagger отображение HealthCheck конечной точки
    /// </summary>
    /// <remarks>
    /// <para>
    /// Обратить внимание, что метод только добавляет описание конечной точки в документацию Swagger.
    /// Для функционирования конечной точки необходимо добавить HealthCheck middleware в конвейер обработки запросов.
    /// В ином случае конечная точка будет неактивна. Соответствующий пункт Swagger UI будет возвращать ошибку 404 Not Found
    /// </para>
    /// </remarks>
    public static IHostApplicationBuilder AddHealthChecksSwagger(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.DocumentFilter<HealthChecksDocumentFilter>();
        });

        return builder;
    }
}
