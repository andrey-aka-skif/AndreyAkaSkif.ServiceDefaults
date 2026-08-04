using System.Net.Http.Headers;

namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.Infrastructure.GitHub;

/// <summary>
/// Типизированный клиент GitHub API
/// </summary>
/// <remarks>
/// <para>
/// Клиент инкапсулирует знание о внешнем API: адреса конечных точек, структуру ответа
/// и требования к запросу. Прикладной код вызывает метод и получает
/// <see cref="GitHubRepository"/>, про HTTP не зная ничего.
/// </para>
/// <para>
/// Интерфейса у клиента нет намеренно: это аналог сгенерированного клиента, и внедряется
/// он конкретным типом. Регистрирует его
/// <c>AddApiClient&lt;GitHubApiClient, GitHubApiClientOptions&gt;()</c>.
/// </para>
/// </remarks>
internal sealed class GitHubApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Создать клиент GitHub API
    /// </summary>
    /// <param name="httpClient">
    /// Клиент с уже подставленным <c>BaseAddress</c> из секции
    /// <c>GitHubApiClientOptions</c>
    /// </param>
    public GitHubApiClient(HttpClient httpClient)
    {
        // GitHub отвечает 403 на запрос без User-Agent. Это часть контракта внешнего API,
        // поэтому заголовок выставляется здесь, а не в composition root: клиент — то самое
        // место, которое знает, чего требует GitHub
        httpClient.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("AndreyAkaSkif.ServiceDefaults.Samples", "1.0"));

        _httpClient = httpClient;
    }

    /// <summary>
    /// Получить сведения о репозитории
    /// </summary>
    /// <param name="owner">Владелец репозитория</param>
    /// <param name="name">Имя репозитория</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Сведения о репозитории</returns>
    public async Task<GitHubRepository?> GetRepositoryAsync(
        string owner,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Адрес конечной точки относительный: абсолютную часть задаёт BaseAddress
        return await _httpClient.GetFromJsonAsync<GitHubRepository>(
            $"repos/{owner}/{name}",
            cancellationToken);
    }
}
