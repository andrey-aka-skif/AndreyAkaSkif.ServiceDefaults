namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.AppSettings;

/// <summary>
/// Настройки приложения. Секция конфигурации называется так же, как тип
/// </summary>
/// <remarks>
/// Чистый POCO: ни интерфейсов, ни атрибутов, ни ссылки на пакет ServiceDefaults.
/// Правила валидации живут отдельно, см. <see cref="DemoAppSettingsValidator"/>.
/// Домену такой тип отдавать не обязательно — сервис объявляет собственный
/// объект-параметр рядом с собой, см. <c>Services/GreetingServiceArgs.cs</c>
/// </remarks>
public sealed record DemoAppSettings
{
    public string Greeting { get; init; } = string.Empty;
}
