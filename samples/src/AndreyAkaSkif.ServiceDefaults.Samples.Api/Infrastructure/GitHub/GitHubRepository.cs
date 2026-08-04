using System.Text.Json.Serialization;

namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.Infrastructure.GitHub;

/// <summary>
/// Репозиторий GitHub — часть ответа <c>GET /repos/{owner}/{name}</c>
/// </summary>
/// <remarks>
/// <para>
/// Тип описывает ровно те поля, которые нужны приложению, а не весь ответ GitHub.
/// Атрибуты <c>JsonPropertyName</c> сопоставляют snake_case внешнего API
/// с именами свойств.
/// </para>
/// <para>
/// Это внутренний тип инфраструктуры, а не контракт сервиса: наружу он не отдаётся —
/// ответ конечной точки собирается отдельно, иначе имена полей GitHub протекли бы
/// в спецификацию OpenAPI образца.
/// </para>
/// </remarks>
/// <param name="FullName">Полное имя репозитория, например <c>"owner/name"</c></param>
/// <param name="Description">Описание репозитория</param>
/// <param name="StargazersCount">Количество звёзд</param>
internal sealed record GitHubRepository(
    [property: JsonPropertyName("full_name")] string FullName,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("stargazers_count")] int StargazersCount);
