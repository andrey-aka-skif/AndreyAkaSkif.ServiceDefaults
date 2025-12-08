using AndreyAkaSkif.ServiceDefaults.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AndreyAkaSkif.ServiceDefaults.Swagger;

/// <summary>
/// Предоставляет методы расширения для настройки OpenAPI через Swagger на основе конфигурации.
/// </summary>
public static class ConfiguredOpenApiViaSwaggerConfigureExtensions
{
    /// <summary>
    /// Добавляет настройки OpenAPI/Swagger в приложение с использованием конфигурации из секции "SwaggerAppSettings".
    /// </summary>
    /// <param name="builder">Построитель приложения.</param>
    /// <returns>Построитель приложения для цепочки вызовов.</returns>
    /// <remarks>
    /// <para>
    /// Требует наличия в конфигурации секции "SwaggerAppSettings" со следующей структурой:
    /// <code>
    /// "SwaggerAppSettings": {
    ///     "Title": "string",            // Название API
    ///     "Description": "string",      // Описание API
    ///     "ApiVersion": "string",       // Версия API
    ///     "Servers": [                  // Массив серверов (URL)
    ///         "string"
    ///     ]
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// В случае отсутствия обязательных параметров или невалидных данных в секции конфигурации
    /// будет выброшено исключение <see cref="ArgumentException"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если секция "SwaggerAppSettings" отсутствует в конфигурации или содержит некорректные данные.
    /// </exception>
    public static IHostApplicationBuilder AddConfiguredOpenApiViaSwagger(this IHostApplicationBuilder builder)
    {
        var settings = builder.Configuration.CreateValidated<SwaggerAppSettings>();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                settings.ApiVersion,
                new OpenApiInfo
                {
                    Title = settings.Title,
                    Version = settings.ApiVersion,
                    Description = settings.Description
                });

            settings.Servers.ForEach(server =>
            {
                options.AddServer(new OpenApiServer { Url = server });
            });
        });

        return builder;
    }

    /// <summary>
    /// Подключает и настраивает промежуточное ПО OpenAPI/Swagger для использования в приложении.
    /// </summary>
    /// <param name="app">Экземпляр веб-приложения.</param>
    /// <returns>Экземпляр веб-приложения для цепочки вызовов.</returns>
    /// <remarks>
    /// <para>
    /// Метод активирует Swagger и Swagger UI только в development среде.
    /// Для корректной работы требуется наличие валидной секции конфигурации "SwaggerAppSettings".
    /// </para>
    /// <para>
    /// Дополнительно настраивает маршрут "/" для автоматического перенаправления на страницу Swagger UI ("/swagger").
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если секция "SwaggerAppSettings" отсутствует в конфигурации или содержит некорректные данные.
    /// </exception>
    public static WebApplication UseConfiguredOpenApiViaSwagger(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return app;

        var settings = app.Configuration.CreateValidated<SwaggerAppSettings>();

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(
                $"/swagger/{settings.ApiVersion}/swagger.json",
                $"specification v{settings.ApiVersion}");
        });

        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

        return app;
    }
}
