namespace AndreyAkaSkif.ServiceDefaults.Settings;

/// <summary>
/// Интерфейс для объектов настроек, поддерживающих валидацию.
/// </summary>
public interface IValidatableSettingsObject
{
    /// <summary>
    /// Выполняет валидацию объекта настроек.
    /// </summary>
    /// <exception cref="ArgumentException">Выбрасывается, если настройки не прошли валидацию.</exception>
    void Validate();
}
