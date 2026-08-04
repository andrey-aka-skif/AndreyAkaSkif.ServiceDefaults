using AndreyAkaSkif.ServiceDefaults.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AndreyAkaSkif.ServiceDefaults.Swagger;

/// <summary>
/// Предоставляет методы расширения для настройки OpenAPI через Swagger на основе конфигурации.
/// </summary>
public static class ConfiguredOpenApiViaSwaggerConfigureExtensions
{
    /// <summary>
    /// Добавляет настройки OpenAPI/Swagger в приложение с использованием конфигурации из секции "SwaggerAppSettings".
    /// </summary>
    /// <remarks>
    /// <para>
    /// Требует наличия в конфигурации секции "SwaggerAppSettings" со следующей структурой:
    /// <code>
    /// "SwaggerAppSettings": {
    ///     "Title": "string",            // Название API
    ///     "Description": "string",      // Описание API
    ///     "ApiVersion": "string",       // Версия API
    ///     "Servers": [                  // Массив адресов серверов, необязательный
    ///         "string"
    ///     ]
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Отсутствующая или некорректная секция роняет приложение при старте хоста,
    /// до первого запроса.
    /// </para>
    /// <para>
    /// Адреса в "Servers" следует задавать относительными ("/", "/api"). Swagger UI берёт
    /// базовый адрес запросов из первого элемента списка, поэтому абсолютный адрес ломает
    /// работу UI везде, кроме прописанного адреса: за reverse proxy, на другом хосте, а также
    /// при открытии страницы по https рядом с http-адресом в конфигурации — браузер режет
    /// такой запрос как mixed content. Относительный адрес допускается OpenAPI 3, и UI
    /// резолвит его от адреса страницы. Если список пуст или отсутствует,
    /// подставляется адрес "/".
    /// </para>
    /// <para>
    /// Настройки регистрируются в конвейере параметров, откуда их берёт
    /// <see cref="UseConfiguredOpenApiViaSwagger"/>.
    /// </para>
    /// <para>
    /// Метод регистрирует ApiExplorer, на котором строится генерация спецификации:
    /// отдельный вызов <c>AddEndpointsApiExplorer()</c> в приложении не требуется.
    /// </para>
    /// <para>
    /// При использовании методов из библиотеки ServiceDefaults.Swagger
    /// всегда устанавливается пакет Swashbuckle.AspNetCore.Annotations.
    /// Даже, если в проекте не используются MVC контроллеры,
    /// а только Minimal Api.
    /// </para>
    /// </remarks>
    /// <param name="builder">Построитель приложения</param>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    public static IHostApplicationBuilder AddConfiguredOpenApiViaSwagger(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AddValidatedOptions<SwaggerAppSettings, SwaggerAppSettingsValidator>();

        // дефолтизация живёт между привязкой и валидацией: порядок
        // configure → postConfigure → validate гарантирован конвейером
        builder.Services.PostConfigure<SwaggerAppSettings>(settings =>
        {
            if (settings.Servers.Count == 0)
                settings.Servers.Add(SwaggerAppSettings.DefaultServer);
        });

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();

        // спецификация собирается лениво: на момент вызова настройки ещё не привязаны
        builder.Services
            .AddOptions<SwaggerGenOptions>()
            .Configure<IOptions<SwaggerAppSettings>>((options, appSettings) =>
            {
                var settings = appSettings.Value;

                options.SwaggerDoc(
                    settings.ApiVersion,
                    new OpenApiInfo
                    {
                        Title = settings.Title,
                        Version = settings.ApiVersion,
                        Description = settings.Description
                    });

                options.EnableAnnotations();

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
    /// Для корректной работы требуется предварительный вызов
    /// <see cref="AddConfiguredOpenApiViaSwagger"/>: настройки берутся из
    /// DI-контейнера, конфигурация повторно не читается.
    /// </para>
    /// <para>
    /// Дополнительно настраивает маршрут "/" для автоматического перенаправления на страницу Swagger UI ("/swagger").
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается, если <see cref="AddConfiguredOpenApiViaSwagger"/> не был вызван.
    /// </exception>
    public static WebApplication UseConfiguredOpenApiViaSwagger(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (!app.Environment.IsDevelopment())
            return app;

        // без парного Add* конвейер отдал бы пустые настройки, поэтому наличие
        // правил валидации проверяется явно
        if (app.Services.GetService<IValidateOptions<SwaggerAppSettings>>() is null)
            throw new InvalidOperationException(
                $"Требуется вызов {nameof(AddConfiguredOpenApiViaSwagger)}()");

        var settings = app.Services.GetRequiredService<IOptions<SwaggerAppSettings>>().Value;

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
