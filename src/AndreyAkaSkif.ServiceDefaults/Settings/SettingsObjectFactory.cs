using Microsoft.Extensions.Configuration;

namespace AndreyAkaSkif.ServiceDefaults.Settings;

/// <summary>
/// Фабрика для создания и валидации объектов настроек.
/// </summary>
public static class SettingsObjectFactory
{
    /// <summary>
    /// Создает экземпляр настроек, заполняет его из конфигурации и выполняет валидацию.
    /// </summary>
    /// <typeparam name="T">Тип создаваемого объекта настроек.</typeparam>
    /// <param name="configuration">Конфигурация, из которой будут взяты значения настроек.</param>
    /// <returns>Валидированный экземпляр настроек типа <typeparamref name="T"/>.</returns>
    /// <exception cref="Exception">Выбрасывается, если настройки не прошли валидацию.</exception>
    /// <remarks>
    /// Секция конфигурации должна иметь имя, соответствующее имени типа <typeparamref name="T"/>.
    /// </remarks>
    public static T CreateValidated<T>(IConfiguration configuration) where T : IValidatableSettingsObject, new()
    {
        var settings = new T();
        configuration.GetSection(typeof(T).Name).Bind(settings);
        settings.Validate();
        return settings;
    }
}
