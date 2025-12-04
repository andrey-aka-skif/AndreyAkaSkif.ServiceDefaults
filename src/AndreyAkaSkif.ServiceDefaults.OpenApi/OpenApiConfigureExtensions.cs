using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.OpenApi;

/// <summary>
/// Предоставляет методы расширения для настройки OpenAPI в приложении.
/// </summary>
public static class OpenApiConfigureExtensions
{
    /// <summary>
    /// Добавляет стандартную конфигурацию OpenAPI в приложение.
    /// </summary>
    /// <remarks>
    /// Для OpenAPI спецификации не доступен UI.
    /// JSON-файл спецификации доступен по пути "/openapi/v1.json".
    /// </remarks>
    /// <param name="builder">Экземпляр <see cref="IHostApplicationBuilder"/></param>
    /// <returns>
    /// Тот же экземпляр <see cref="IHostApplicationBuilder"/> для поддержки цепочки вызовов.
    /// </returns>
    public static IHostApplicationBuilder AddDefaultOpenApi(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();

        return builder;
    }

    /// <summary>
    /// Использование конечной точки OpenApi.
    /// </summary>
    /// <remarks>
    /// Этот метод формирует конечную точку OpenAPI только в том случае,
    /// если приложение запущено в development среде.
    /// В production или иной среде конечная точка не формируется.
    /// </remarks>
    /// <param name="app">Экземпляр <see cref="WebApplication"/>.</param>
    /// <returns>Тот же экземпляр <see cref="IApplicationBuilder"/> для поддержки цепочки вызовов.</returns>
    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.MapOpenApi();

        return app;
    }
}
