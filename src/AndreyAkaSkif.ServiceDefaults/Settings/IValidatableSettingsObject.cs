namespace AndreyAkaSkif.ServiceDefaults.Settings;

/// <summary>
/// Интерфейс для объектов настроек, поддерживающих валидацию.
/// </summary>
[Obsolete("Правила валидации выносятся в отдельный IValidateOptions<T>, " +
          "класс настроек остаётся чистым POCO — см. AddAppSettings<T, TValidator>()")]
public interface IValidatableSettingsObject
{
    /// <summary>
    /// Выполняет валидацию объекта настроек.
    /// </summary>
    /// <exception cref="ArgumentException">Выбрасывается, если настройки не прошли валидацию.</exception>
    void Validate();
}
