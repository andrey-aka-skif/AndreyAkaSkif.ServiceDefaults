using AndreyAkaSkif.ServiceDefaults.HttpApiClients;
using AndreyAkaSkif.ServiceDefaults.Samples.Api.Infrastructure.GitHub;

namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.AppConfiguration;

/// <summary>
/// Типизированные API-клиенты приложения
/// </summary>
internal static class AppHttpClientsExtensions
{
    /// <summary>
    /// Зарегистрировать типизированные API-клиенты
    /// </summary>
    /// <remarks>
    /// <para>
    /// Сюда складываются регистрации клиентов внешних API. Каждый вызов —
    /// это пара «клиент и его настройки»: адрес читается из одноимённой типу настроек
    /// секции конфигурации и подставляется в <c>HttpClient</c>.
    /// </para>
    /// <para>
    /// Реализация клиента живёт в каталоге <c>Infrastructure</c> — это адаптер
    /// к внешнему миру, а не логика приложения. Всё, что специфично для внешнего API —
    /// адреса конечных точек, обязательные заголовки, структура ответа, — принадлежит
    /// самому клиенту, поэтому здесь остаётся одна строка.
    /// </para>
    /// <para>
    /// В отличие от <c>AddConfiguredCorsPolicy()</c> и прочих вызовов пакетов, которые
    /// в образце оставлены в <c>Program.cs</c> на виду, этот вызов вынесен сюда: он несёт
    /// прикладные аргументы-типы и растёт вместе с приложением.
    /// </para>
    /// </remarks>
    public static IHostApplicationBuilder AddAppHttpClients(this IHostApplicationBuilder builder)
    {
        builder.AddApiClient<GitHubApiClient, GitHubApiClientOptions>();

        return builder;
    }
}
