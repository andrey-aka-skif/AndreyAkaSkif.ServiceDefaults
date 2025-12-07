using AndreyAkaSkif.ServiceDefaults.Settings;
using Microsoft.AspNetCore.Builder;

namespace AndreyAkaSkif.ServiceDefaults.Routing;

/// <summary>
/// Методы расширения для регистрации middleware, добавляющего базовый путь в DI-контейнере.
/// </summary>
public static class RouteAppSettingsExtensions
{
    /// <summary>
    /// Добавить базовый путь на основе конфигурации
    /// </summary>
    /// <remarks>
    /// <para>
    /// Требует секцию конфигурации "RouteAppSettings" следующего вида:
    /// <code>
    /// "RouteAppSettings": {
    ///   "PathBase": "/api"
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Параметр <see cref="RouteAppSettings.PathBase"/> должен быть валидно сформированной Uri-строкой
    /// и начинаться с лидирующего "/"
    /// </para>
    /// <para>
    /// Для базового пути <strong>запрешены</strong> следующие символы:
    /// <code>
    /// '?', '#', '&lt;', '&gt;', '[', ']', '(', ')', '^', '`', '|', '\', ':', '*', '"', ''', '%', '!', '@', ' '
    /// </code>
    /// </para>
    /// <para>
    /// <strong>Важно:</strong>
    /// <list type="bullet">
    /// <item>Базовый путь не заменяет основной путь</item>
    /// <item>Основной путь также доступен для использования</item>
    /// <item>Указанный базовый путь не будет автоматически добавлен в спецификацию OpenApi</item>
    /// </list>
    /// </para>
    /// <para>
    /// Пример явного добавления базового пути ("/api") в спецификацию OpenApi при конфигурировании Swagger:
    /// <code>
    /// builder.Services.AddSwaggerGen(options =>
    /// {
    ///     options.SwaggerDoc(/**/);
    ///     options.AddServer(new OpenApiServer { Url = "http://127.0.0.1:5005/api" });
    /// });
    /// </code>
    /// </para>
    /// </remarks>
    public static WebApplication UseConfiguredPathBase(this WebApplication app)
    {
        var routeAppSettings = app.Configuration.CreateValidated<RouteAppSettings>();
        app.UsePathBase(routeAppSettings.PathBase);

        return app;
    }
}
