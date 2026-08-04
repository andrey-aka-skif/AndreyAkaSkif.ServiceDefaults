using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.HttpApiClients;

/// <summary>
/// Правила валидации настроек типизированного API-клиента
/// </summary>
/// <typeparam name="TOptions">Тип настроек клиента</typeparam>
internal sealed class HttpApiClientOptionsValidator<TOptions> : IValidateOptions<TOptions>
    where TOptions : class, IHttpApiClientOptions
{
    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // валидатор активируется контейнером и фактического имени секции не знает,
        // поэтому в сообщении называется тип настроек — тот, с которым вызывали AddApiClient
        return IsHttpAddress(options.BaseAddress)
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(
                $"Требуется {typeof(TOptions).Name}:{nameof(IHttpApiClientOptions.BaseAddress)} — " +
                $"абсолютный http- или https-адрес вида \"https://api.example.com/\"");
    }

    /// <summary>
    /// Проверить, что значение является абсолютным http- или https-адресом
    /// </summary>
    private static bool IsHttpAddress(string? value)
        // Одного UriKind.Absolute мало: в Unix путь вида "/api" разбирается как путь
        // файловой системы и даёт валидный file:///api, а в Windows — не разбирается
        // вовсе. Схема проверяется явно, иначе поведение зависело бы от платформы
        => Uri.TryCreate(value, UriKind.Absolute, out var uri)
           && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
