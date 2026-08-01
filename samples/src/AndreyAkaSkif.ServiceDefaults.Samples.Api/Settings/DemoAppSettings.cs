using AndreyAkaSkif.ServiceDefaults.Settings;

namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.Settings;

/// <summary>
/// Настройки приложения. Секция конфигурации называется так же, как тип
/// </summary>
public sealed record DemoAppSettings : IValidatableSettingsObject
{
    public string Greeting { get; init; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Greeting))
            throw new ArgumentException($"Требуется {nameof(DemoAppSettings)}:{nameof(Greeting)}");
    }
}
