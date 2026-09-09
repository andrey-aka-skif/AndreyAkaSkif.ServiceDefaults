using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Swagger;

/// <summary>
/// Предоставляет методы расширения для показа спецификации OpenAPI через Swagger UI
/// </summary>
/// <remarks>
/// Пакет только показывает спецификацию и не участвует в её создании: чем и где документ
/// сгенерирован, ему безразлично — задан лишь адрес, по которому документ доступен.
/// </remarks>
public static class SwaggerUiConfigureExtensions
{
    private const string SwaggerUiPath = "/swagger";

    /// <summary>
    /// Добавляет настройки Swagger UI из секции "Swagger"
    /// </summary>
    /// <remarks>
    /// <para>
    /// Секция конфигурации целиком необязательна:
    /// <code>
    /// "Swagger": {
    ///     "Url": "string",        // Адрес спецификации, по умолчанию "/openapi/v1.json"
    ///     "Name": "string",       // Имя документа в UI, по умолчанию — значение "Url"
    ///     "Visibility": "string"  // ByEnvironment | Always | Never
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Заданный пустым "Url" роняет приложение при старте хоста, до первого запроса:
    /// UI поднялся бы, но спецификацию не нашёл.
    /// </para>
    /// <para>
    /// Спецификацию по адресу "Url" должно раздавать само приложение — например пакетом
    /// <c>AndreyAkaSkif.ServiceDefaults.OpenApi</c> — либо сторонний сервис. Показ UI и
    /// раздача документа управляются независимо, поэтому в среде, где документ недоступен,
    /// UI откроется пустым.
    /// </para>
    /// </remarks>
    /// <param name="builder">Построитель приложения</param>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    public static IHostApplicationBuilder AddSwaggerUi(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IValidateOptions<SwaggerAppSettings>, SwaggerAppSettingsValidator>());

        builder.Services
            .AddOptions<SwaggerAppSettings>()
            .BindConfiguration(SwaggerAppSettings.SectionName)
            .ValidateOnStart();

        return builder;
    }

    /// <summary>
    /// Подключает Swagger UI для спецификации, заданной секцией "Swagger"
    /// </summary>
    /// <remarks>
    /// <para>
    /// Требует предварительного вызова <see cref="AddSwaggerUi"/>: настройки берутся
    /// из DI-контейнера, конфигурация повторно не читается.
    /// </para>
    /// <para>
    /// Показ определяет ключ "Swagger:Visibility": "ByEnvironment" (значение по умолчанию,
    /// UI показывается везде, кроме Production), "Always" или "Never".
    /// </para>
    /// <para>
    /// Дополнительно настраивает маршрут "/" для перенаправления на страницу Swagger UI
    /// ("/swagger"). В среде, где UI не показывается, ни страница, ни перенаправление
    /// не формируются.
    /// </para>
    /// </remarks>
    /// <param name="app">Экземпляр веб-приложения.</param>
    /// <returns>Тот же экземпляр <paramref name="app"/> для поддержки цепочки вызовов.</returns>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается, если <see cref="AddSwaggerUi"/> не был вызван.
    /// </exception>
    public static WebApplication UseSwaggerUi(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // без парного Add* конвейер отдал бы пустые настройки, поэтому наличие
        // правил валидации проверяется явно
        if (app.Services.GetService<IValidateOptions<SwaggerAppSettings>>() is null)
            throw new InvalidOperationException($"Требуется вызов {nameof(AddSwaggerUi)}()");

        var settings = app.Services.GetRequiredService<IOptions<SwaggerAppSettings>>().Value;

        if (!settings.Visibility.IsVisibleIn(app.Environment))
            return app;

        // документ Swashbuckle не принадлежит, поэтому адрес указывается явно
        app.UseSwaggerUI(options => options.SwaggerEndpoint(settings.Url, settings.DisplayName));

        app.MapGet("/", () => Results.Redirect(SwaggerUiPath)).ExcludeFromDescription();

        return app;
    }
}
