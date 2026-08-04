using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.AppSettings;

/// <summary>
/// Правила валидации <see cref="DemoAppSettings"/>
/// </summary>
/// <remarks>
/// Валидатор активируется контейнером, поэтому при необходимости может принимать
/// зависимости через конструктор. Нарушения накапливаются: конвейер параметров
/// покажет весь список, а не первое попавшееся
/// </remarks>
internal sealed class DemoAppSettingsValidator : IValidateOptions<DemoAppSettings>
{
    public ValidateOptionsResult Validate(string? name, DemoAppSettings options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return string.IsNullOrWhiteSpace(options.Greeting)
            ? ValidateOptionsResult.Fail(
                $"Требуется {nameof(DemoAppSettings)}:{nameof(DemoAppSettings.Greeting)}")
            : ValidateOptionsResult.Success;
    }
}
