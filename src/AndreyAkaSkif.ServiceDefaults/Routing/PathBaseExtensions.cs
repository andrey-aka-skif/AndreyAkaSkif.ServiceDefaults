using AndreyAkaSkif.ServiceDefaults.Settings;
using Microsoft.AspNetCore.Builder;

namespace AndreyAkaSkif.ServiceDefaults.Routing;

/// <summary>
/// Методы расширения для регистрации middleware, добавляющего базовый путь в DI-контейнере.
/// </summary>
public static class PathBaseExtensions
{
    /// <summary>
    /// Добавить базовый путь на основе конфигурации
    /// </summary>
    /// <remarks>
    /// <para>
    /// Требует секцию конфигурации "PathBaseAppSettings" следующего вида:
    /// <code>
    /// "PathBaseAppSettings": {
    ///   "Path": "/api"
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Допустимый формат <see cref="PathBaseAppSettings.Path"/> — пустая строка либо путь
    /// вида "/segment[/segment...]", где сегмент состоит из unreserved-символов RFC 3986:
    /// <code>
    /// A-Za-z0-9 - . _ ~
    /// </code>
    /// Пустая строка означает, что базовый путь не добавляется. Хвостовой слеш допустим —
    /// <c>UsePathBase</c> срезает его сам.
    /// </para>
    /// <para>
    /// Percent-encoding не поддерживается: базовый путь сравнивается с уже раскодированным
    /// путём запроса, поэтому "%D0%B0" останется буквальными символами. Не-ASCII символы
    /// также запрещены — базовый путь попадает в спецификацию OpenApi, в конфигурацию
    /// обратного прокси и в логи, где становится источником двойного кодирования.
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
    /// <exception cref="ArgumentException">
    /// если значение <see cref="PathBaseAppSettings.Path"/> не проходит валидацию
    /// </exception>
    public static WebApplication UseConfiguredPathBase(this WebApplication app)
    {
        var settings = app.Configuration.CreateValidated<PathBaseAppSettings>();
        app.UsePathBase(settings.Path);

        return app;
    }
}
