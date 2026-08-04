using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.HttpApiClients;

/// <summary>
/// Методы расширения для регистрации типизированных API-клиентов
/// </summary>
public static class HttpApiClientExtensions
{
    /// <summary>
    /// Зарегистрировать типизированный API-клиент с адресом из конфигурации
    /// </summary>
    /// <typeparam name="TClient">
    /// Тип клиента. Принимает <c>HttpClient</c> через конструктор и регистрируется
    /// в DI-контейнере как transient
    /// </typeparam>
    /// <typeparam name="TOptions">Тип настроек клиента</typeparam>
    /// <param name="builder">Построитель приложения</param>
    /// <param name="sectionName">
    /// Имя секции конфигурации. По умолчанию — имя типа настроек: для
    /// <c>GitHubApiClientOptions</c> это <c>"GitHubApiClientOptions"</c>
    /// </param>
    /// <remarks>
    /// <para>
    /// API-клиент — инфраструктурный адаптер. Он инкапсулирует знание о внешнем API:
    /// адреса конечных точек, структуру JSON, типы ответов и требования вроде обязательных
    /// заголовков. Прикладной код работает с ним как с обычным сервисом и про HTTP не знает.
    /// </para>
    /// <para>
    /// Метод регистрирует настройки в конвейере параметров, проверяет, что
    /// <see cref="IHttpApiClientOptions.BaseAddress"/> задан и является абсолютным адресом,
    /// и подставляет его в <c>HttpClient.BaseAddress</c> клиента:
    /// <code>
    /// public sealed class GitHubApiClientOptions : IHttpApiClientOptions
    /// {
    ///     public string BaseAddress { get; set; } = string.Empty;
    /// }
    ///
    /// internal sealed class GitHubApiClient(HttpClient httpClient)
    /// {
    ///     public Task&lt;GitHubRepository?&gt; GetRepositoryAsync(string owner, string name)
    ///         =&gt; httpClient.GetFromJsonAsync&lt;GitHubRepository&gt;($"repos/{owner}/{name}");
    /// }
    ///
    /// builder.AddApiClient&lt;GitHubApiClient, GitHubApiClientOptions&gt;();
    /// </code>
    /// Секция конфигурации в этом случае:
    /// <code>
    /// "GitHubApiClientOptions": {
    ///     "BaseAddress": "https://api.github.com/"
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Некорректный или отсутствующий адрес роняет приложение при старте хоста —
    /// <c>ValidateOnStart</c>, — а не при первом запросе к внешнему API. Это чуть позже,
    /// чем у остальных <c>Add*</c>-методов пакета: те падают до <c>Build()</c>.
    /// </para>
    /// <para>
    /// Значение читается один раз, при создании клиента. Изменение конфигурации на лету
    /// на уже созданные клиенты не влияет.
    /// </para>
    /// <para>
    /// Заголовки, обработчики и прочая настройка клиента в сигнатуру не вынесены: метод —
    /// готовое решение, а не конструктор. Знание о внешнем API принадлежит самому клиенту,
    /// поэтому обязательные заголовки выставляются в его конструкторе. Если приложению
    /// нужен <c>DelegatingHandler</c> или политика устойчивости, регистрация дополняется
    /// штатным API — <c>AddHttpClient</c> аддитивен:
    /// <code>
    /// builder.AddApiClient&lt;GitHubApiClient, GitHubApiClientOptions&gt;();
    ///
    /// builder.Services
    ///        .AddHttpClient&lt;GitHubApiClient&gt;()
    ///        .AddHttpMessageHandler&lt;AuthorizationHandler&gt;();
    /// </code>
    /// </para>
    /// </remarks>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    /// <exception cref="ArgumentException">
    /// если <paramref name="sectionName"/> задан и пуст
    /// </exception>
    public static IHostApplicationBuilder AddApiClient<TClient, TOptions>(
        this IHostApplicationBuilder builder,
        string? sectionName = null)
        where TClient : class
        where TOptions : class, IHttpApiClientOptions
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (sectionName is not null && string.IsNullOrWhiteSpace(sectionName))
            throw new ArgumentException("Не задано имя секции конфигурации", nameof(sectionName));

        var section = sectionName ?? typeof(TOptions).Name;

        builder.Services
            .AddOptions<TOptions>()
            .BindConfiguration(section)
            .Validate(
                options =>
                {
                    return Uri.TryCreate(options.BaseAddress, UriKind.Absolute, out _);
                },
                $"Требуется {section}:{nameof(IHttpApiClientOptions.BaseAddress)} — " +
                $"абсолютный адрес вида \"https://api.example.com/\"")
            .ValidateOnStart();

        builder.Services.AddHttpClient<TClient>((serviceProvider, client) =>
            client.BaseAddress = new Uri(
                serviceProvider.GetRequiredService<IOptions<TOptions>>().Value.BaseAddress,
                UriKind.Absolute));

        return builder;
    }
}
