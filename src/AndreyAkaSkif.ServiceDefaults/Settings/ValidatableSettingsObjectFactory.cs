using Microsoft.Extensions.Configuration;

namespace AndreyAkaSkif.ServiceDefaults.Settings;

/// <summary>
/// Фабрика для создания и валидации объектов настроек
/// </summary>
public static class ValidatableSettingsObjectFactory
{
    /// <summary>
    /// Создает экземпляр настроек, заполняет его из конфигурации хоста и выполняет валидацию.
    /// </summary>
    /// <typeparam name="T">
    /// Тип создаваемого объекта настроек, реализующий <see cref="IValidatableSettingsObject"/>.
    /// </typeparam>
    /// <returns>
    /// Объект настроек типа <see cref="IValidatableSettingsObject"/>, прошедший валидацию.
    /// </returns>
    public static T CreateValidated<T>(this IConfiguration builder) where T : IValidatableSettingsObject, new()
    {
        var settings = new T();
        builder.GetSection(typeof(T).Name).Bind(settings);
        settings.Validate();
        return settings;
    }
}
