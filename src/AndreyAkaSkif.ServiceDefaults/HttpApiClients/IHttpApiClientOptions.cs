namespace AndreyAkaSkif.ServiceDefaults.HttpApiClients;

/// <summary>
/// Минимальный контракт настроек типизированного API-клиента
/// </summary>
/// <remarks>
/// <para>
/// Единственное, что библиотека обязана знать о настройках клиента, — адрес внешнего API.
/// Всё остальное — ключи, версии, идентификаторы — принадлежит приложению, объявляется
/// в том же классе настроек и библиотеку не касается.
/// </para>
/// <para>
/// Интерфейс, а не базовый класс: наследование не навязывается, а требование проверяется
/// компилятором в месте вызова
/// <see cref="HttpApiClientExtensions.AddApiClient{TClient, TOptions}"/>.
/// </para>
/// <para>
/// Пример настроек клиента:
/// <code>
/// public sealed class GitHubApiClientOptions : IHttpApiClientOptions
/// {
///     public string BaseAddress { get; set; } = string.Empty;
/// }
/// </code>
/// Свойство объявляется с <c>set</c>: значение в него кладёт привязка конфигурации.
/// </para>
/// </remarks>
public interface IHttpApiClientOptions
{
    /// <summary>
    /// Абсолютный адрес внешнего API, например <c>"https://api.example.com/"</c>
    /// </summary>
    /// <remarks>
    /// Значение попадает в <c>HttpClient.BaseAddress</c>. Хвостовой слеш имеет значение:
    /// без него последний сегмент адреса будет отброшен при разрешении относительного пути
    /// запроса — так устроен <see cref="Uri"/>, а не библиотека.
    /// </remarks>
    string BaseAddress { get; }
}
