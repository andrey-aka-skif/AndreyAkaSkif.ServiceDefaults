using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.Swagger;

/// <summary>
/// Предоставляет методы расширения для настройки OpenAPI через Swagger.
/// </summary>
public static class DefaultOpenApiViaSwaggerConfigureExtensions
{
    /// <summary>
    /// Добавляет стандартную конфигурацию OpenAPI с использованием Swagger.
    /// </summary>
    /// <remarks>
    /// Swagger UI доступен по адресу "/swagger/index.html".
    /// JSON-файл спецификации доступен по пути "/swagger/v1/swagger.json".
    /// </remarks>
    /// <param name="builder">Экземпляр <see cref="IHostApplicationBuilder"/></param>
    /// <returns>
    /// Тот же экземпляр <see cref="IHostApplicationBuilder"/> для поддержки цепочки вызовов.
    /// </returns>
    public static IHostApplicationBuilder AddDefaultOpenApiViaSwagger(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();

        return builder;
    }

    /// <summary>
    /// Использование конечной точки OpenApi для отображения Swagger UI.
    /// </summary>
    /// <remarks>
    /// Метод формирует конечную точку Swagger UI только в том случае,
    /// если приложение запущено в development среде.
    /// В production или иной среде конечная точка не формируется.
    /// </remarks>
    /// <param name="app">Экземпляр <see cref="WebApplication"/>.</param>
    /// <returns>Тот же экземпляр <see cref="IApplicationBuilder"/> для поддержки цепочки вызовов.</returns>
    public static IApplicationBuilder UseDefaultOpenApiViaSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}
