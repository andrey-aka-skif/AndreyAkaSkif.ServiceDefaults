using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Swagger;

internal sealed class SwaggerAppSettingsValidator : IValidateOptions<SwaggerAppSettings>
{
    public ValidateOptionsResult Validate(string? name, SwaggerAppSettings options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // отсутствующий ключ подставляет умолчание, поэтому проверять остаётся только
        // заданное пустым значение: с ним UI поднялся бы, но не нашёл спецификацию
        return string.IsNullOrWhiteSpace(options.Url)
            ? ValidateOptionsResult.Fail(
                $"Требуется {SwaggerAppSettings.SectionName}:{nameof(SwaggerAppSettings.Url)}")
            : ValidateOptionsResult.Success;
    }
}
