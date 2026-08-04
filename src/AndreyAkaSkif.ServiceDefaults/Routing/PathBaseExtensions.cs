using AndreyAkaSkif.ServiceDefaults.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Routing;

/// <summary>
/// Методы расширения для регистрации middleware, добавляющего базовый путь в DI-контейнере.
/// </summary>
public static class PathBaseExtensions
{
    /// <summary>
    /// Зарегистрировать настройки базового пути
    /// </summary>
    /// <remarks>
    /// <para>
    /// Требует секцию конфигурации "PathBaseAppSettings" следующего вида:
    /// <code>
    /// "PathBaseAppSettings": {
    ///   "Path": "/api"
    /// }
    /// </code>
    /// Секция необязательна: без неё базовый путь пуст и не добавляется.
    /// </para>
    /// <para>
    /// Допустимый формат <see cref="PathBaseAppSettings.Path"/> — пустая строка либо путь
    /// вида "/segment[/segment...]", где сегмент состоит из unreserved-символов RFC 3986:
    /// <code>
    /// A-Za-z0-9 - . _ ~
    /// </code>
    /// Хвостовой слеш допустим — <c>UsePathBase</c> срезает его сам.
    /// </para>
    /// <para>
    /// Percent-encoding не поддерживается: базовый путь сравнивается с уже раскодированным
    /// путём запроса, поэтому "%D0%B0" останется буквальными символами. Не-ASCII символы
    /// также запрещены — базовый путь попадает в спецификацию OpenApi, в конфигурацию
    /// обратного прокси и в логи, где становится источником двойного кодирования.
    /// </para>
    /// <para>
    /// Некорректное значение роняет приложение при старте хоста, до первого запроса.
    /// Подключение в конвейер — <see cref="UseConfiguredPathBase"/>.
    /// </para>
    /// </remarks>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    public static IHostApplicationBuilder AddConfiguredPathBase(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AddValidatedOptions<PathBaseAppSettings, PathBaseAppSettingsValidator>();

        return builder;
    }

    /// <summary>
    /// Добавить базовый путь на основе конфигурации
    /// </summary>
    /// <remarks>
    /// <para>
    /// Требует предварительного вызова <see cref="AddConfiguredPathBase"/>:
    /// настройки берутся из DI-контейнера, конфигурация повторно не читается.
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
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается, если <see cref="AddConfiguredPathBase"/> не был вызван.
    /// </exception>
    public static WebApplication UseConfiguredPathBase(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // без парного Add* конвейер отдал бы пустой путь и саброутинг молча
        // перестал бы работать, поэтому наличие правил валидации проверяется явно
        if (app.Services.GetService<IValidateOptions<PathBaseAppSettings>>() is null)
            throw new InvalidOperationException(
                $"Требуется вызов {nameof(AddConfiguredPathBase)}()");

        var settings = app.Services.GetRequiredService<IOptions<PathBaseAppSettings>>().Value;
        app.UsePathBase(settings.Path);

        return app;
    }
}
