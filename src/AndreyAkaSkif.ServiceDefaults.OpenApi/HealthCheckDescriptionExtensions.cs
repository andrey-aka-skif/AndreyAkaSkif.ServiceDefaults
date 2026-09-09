using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.OpenApi;

/// <summary>
/// Предоставляет методы расширения для описания конечной точки проверки жизнеспособности
/// в спецификации
/// </summary>
public static class HealthCheckDescriptionExtensions
{
    /// <summary>
    /// Адрес конечной точки проверки жизнеспособности по умолчанию
    /// </summary>
    /// <remarks>
    /// Совпадает с адресом, который регистрирует <c>MapHealthCheckEndpoint()</c> из пакета
    /// <c>AndreyAkaSkif.ServiceDefaults</c>. Значение продублировано литералом сознательно:
    /// зависимости от базового пакета у этого пакета нет
    /// </remarks>
    public const string DefaultHealthCheckEndpoint = "/health";

    /// <summary>
    /// Добавляет в спецификацию описание конечной точки проверки жизнеспособности
    /// </summary>
    /// <remarks>
    /// <para>
    /// В документ добавляется конечная точка <paramref name="endpoint"/> с единственным
    /// ответом <c>200 Healthy</c>. Метод только описывает конечную точку: чтобы она работала,
    /// требуются HealthCheck сервисы и HealthCheck middleware в конвейере обработки запросов.
    /// В ином случае конечная точка вернёт <c>404 Not Found</c>
    /// </para>
    /// <para>
    /// Описание добавляется вручную потому, что конечную точку регистрирует промежуточное ПО,
    /// а не обработчик minimal API: ApiExplorer её не видит, и генератор о ней не знает
    /// </para>
    /// <para>
    /// Метод дополняет любую из пар настройки спецификации — как <c>AddDefaultOpenApi()</c>,
    /// так и <c>AddConfiguredOpenApi()</c>
    /// </para>
    /// </remarks>
    /// <param name="builder">Построитель приложения</param>
    /// <param name="endpoint">
    /// Адрес конечной точки. Должен совпадать с адресом, по которому её регистрирует
    /// приложение
    /// </param>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    /// <exception cref="ArgumentException">если <paramref name="endpoint"/> пуст</exception>
    public static IHostApplicationBuilder AddHealthCheckEndpointDescription(
        this IHostApplicationBuilder builder,
        string endpoint = DefaultHealthCheckEndpoint)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);

        var documentName = OpenApiConfigurationReader.ReadDocumentName(builder.Configuration);

        builder.Services.AddOpenApi(
            documentName,
            options => options.AddDocumentTransformer(new HealthCheckDocumentTransformer(endpoint)));

        return builder;
    }
}
