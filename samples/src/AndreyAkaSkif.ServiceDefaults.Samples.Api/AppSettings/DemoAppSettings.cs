using AndreyAkaSkif.ServiceDefaults.Settings;

namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.AppSettings;

/// <summary>
/// Настройки приложения. Секция конфигурации называется так же, как тип
/// </summary>
/// <remarks>
/// Объект границы приложения: знает имя секции конфигурации и реализует
/// <see cref="IValidatableSettingsObject"/>. Домену такой тип отдавать не обязательно —
/// сервис объявляет собственный объект-параметр рядом с собой, см.
/// <c>Services/GreetingServiceArgs.cs</c>
/// </remarks>
public sealed record DemoAppSettings : IValidatableSettingsObject
{
    public string Greeting { get; init; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Greeting))
            throw new ArgumentException($"Требуется {nameof(DemoAppSettings)}:{nameof(Greeting)}");
    }
}
