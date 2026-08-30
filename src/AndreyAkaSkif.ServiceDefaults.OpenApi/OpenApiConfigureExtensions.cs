using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
    /// <para>
    /// Атрибуция документа не задаётся: заголовок генератор выводит из имени сборки,
    /// версию — из имени документа. Чтобы задать их конфигурацией, используйте
    /// <see cref="AddConfiguredOpenApi"/>.
    /// </para>
    /// <para>
    /// Для OpenAPI спецификации не доступен UI. JSON-файл спецификации доступен
    /// по пути "/openapi/v1.json".
    /// </para>
    /// </remarks>
    /// <param name="builder">Экземпляр <see cref="IHostApplicationBuilder"/></param>
    /// <returns>
    /// Тот же экземпляр <see cref="IHostApplicationBuilder"/> для поддержки цепочки вызовов.
    /// </returns>
    public static IHostApplicationBuilder AddDefaultOpenApi(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddOpenApi();

        return builder;
    }

    /// <summary>
    /// Использование конечной точки OpenApi.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Доступность спецификации определяет ключ "OpenApi:Visibility": "ByEnvironment"
    /// (значение по умолчанию, спецификация доступна везде, кроме Production), "Always"
    /// или "Never". Ключ необязателен, секция целиком — тоже.
    /// </para>
    /// <para>
    /// В среде, где спецификация недоступна, конечная точка не формируется и запрос
    /// к ней возвращает 404.
    /// </para>
    /// </remarks>
    /// <param name="app">Экземпляр <see cref="WebApplication"/>.</param>
    /// <returns>Тот же экземпляр <see cref="WebApplication"/> для поддержки цепочки вызовов.</returns>
    public static WebApplication UseDefaultOpenApi(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var visibility = OpenApiConfigurationReader.ReadVisibility(app.Configuration);

        if (visibility.IsVisibleIn(app.Environment))
            app.MapOpenApi();

        return app;
    }

    /// <summary>
    /// Добавляет конфигурацию OpenAPI с атрибуцией документа из секции "OpenApi".
    /// </summary>
    /// <remarks>
    /// <para>
    /// Секция конфигурации:
    /// <code>
    /// "OpenApi": {
    ///     "Title": "string",            // Заголовок документа, обязателен
    ///     "Description": "string",      // Описание документа, необязательно
    ///     "DocumentName": "string",     // Имя документа, по умолчанию "v1"
    ///     "Version": "string",          // Версия контракта API, по умолчанию "1.0"
    ///     "Servers": [ "string" ],      // Адреса серверов, необязательно
    ///     "Visibility": "string"        // ByEnvironment | Always | Never
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Отсутствующий "Title" или некорректная секция роняют приложение при старте хоста,
    /// до первого запроса.
    /// </para>
    /// <para>
    /// Имя документа попадает в адрес спецификации ("/openapi/{DocumentName}.json") и не
    /// связано ни с версией контракта, ни с версией сборки: адрес, по которому генерируются
    /// клиенты, при выпусках меняться не должен.
    /// </para>
    /// <para>
    /// Адреса в "Servers" следует задавать относительными ("/", "/api"): абсолютный адрес
    /// ломает работу везде, кроме прописанного адреса — за reverse proxy, на другом хосте,
    /// а также при открытии страницы по https рядом с http-адресом в конфигурации. Если
    /// список пуст или отсутствует, подставляется адрес "/".
    /// </para>
    /// </remarks>
    /// <param name="builder">Построитель приложения</param>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    public static IHostApplicationBuilder AddConfiguredOpenApi(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IValidateOptions<OpenApiAppSettings>, OpenApiAppSettingsValidator>());

        builder.Services
            .AddOptions<OpenApiAppSettings>()
            .BindConfiguration(OpenApiAppSettings.SectionName)
            .ValidateOnStart();

        // дефолтизация живёт между привязкой и валидацией: порядок
        // configure → postConfigure → validate гарантирован конвейером
        builder.Services.PostConfigure<OpenApiAppSettings>(settings =>
        {
            if (settings.Servers.Count == 0)
                settings.Servers.Add(OpenApiAppSettings.DefaultServer);
        });

        var documentName = OpenApiConfigurationReader.ReadDocumentName(builder.Configuration);

        // атрибуция применяется трансформером, а не здесь: на момент регистрации
        // настройки ещё не привязаны
        builder.Services.AddOpenApi(
            documentName,
            options => options.AddDocumentTransformer<ApiInfoDocumentTransformer>());

        return builder;
    }

    /// <summary>
    /// Использование конечной точки OpenApi, настроенной секцией "OpenApi".
    /// </summary>
    /// <remarks>
    /// <para>
    /// Требует предварительного вызова <see cref="AddConfiguredOpenApi"/>: настройки берутся
    /// из DI-контейнера, конфигурация повторно не читается.
    /// </para>
    /// <para>
    /// Доступность спецификации определяет ключ "OpenApi:Visibility".
    /// </para>
    /// </remarks>
    /// <param name="app">Экземпляр <see cref="WebApplication"/>.</param>
    /// <returns>Тот же экземпляр <see cref="WebApplication"/> для поддержки цепочки вызовов.</returns>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается, если <see cref="AddConfiguredOpenApi"/> не был вызван.
    /// </exception>
    public static WebApplication UseConfiguredOpenApi(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // без парного Add* конвейер отдал бы пустые настройки, поэтому наличие
        // правил валидации проверяется явно
        if (app.Services.GetService<IValidateOptions<OpenApiAppSettings>>() is null)
            throw new InvalidOperationException(
                $"Требуется вызов {nameof(AddConfiguredOpenApi)}()");

        var settings = app.Services.GetRequiredService<IOptions<OpenApiAppSettings>>().Value;

        if (settings.Visibility.IsVisibleIn(app.Environment))
            app.MapOpenApi();

        return app;
    }
}
