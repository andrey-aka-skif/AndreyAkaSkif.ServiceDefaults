using AndreyAkaSkif.ServiceDefaults.HttpApiClients;

namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.Infrastructure.GitHub;

/// <summary>
/// Настройки клиента GitHub API
/// </summary>
/// <remarks>
/// <para>
/// Секция конфигурации называется по имени типа — <c>GitHubApiClientOptions</c>.
/// Привязку, проверку и подстановку адреса в клиент выполняет
/// <c>AddApiClient&lt;GitHubApiClient, GitHubApiClientOptions&gt;()</c>.
/// </para>
/// <para>
/// Класс лежит рядом с клиентом, а не в каталоге <c>AppSettings</c>: там собраны настройки
/// границы приложения, а эти принадлежат инфраструктурному адаптеру и в домен не попадают —
/// клиенту они достаются как <c>IOptions&lt;T&gt;</c>.
/// </para>
/// </remarks>
internal sealed class GitHubApiClientOptions : IHttpApiClientOptions
{
    /// <summary>
    /// Адрес GitHub API
    /// </summary>
    public string BaseAddress { get; set; } = string.Empty;
}
