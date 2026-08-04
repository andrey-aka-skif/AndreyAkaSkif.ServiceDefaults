using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Swagger;

internal sealed class SwaggerAppSettingsValidator : IValidateOptions<SwaggerAppSettings>
{
    public ValidateOptionsResult Validate(string? name, SwaggerAppSettings options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Title))
            failures.Add($"Требуется {nameof(SwaggerAppSettings)}:{nameof(SwaggerAppSettings.Title)}");

        if (string.IsNullOrWhiteSpace(options.Description))
            failures.Add($"Требуется {nameof(SwaggerAppSettings)}:{nameof(SwaggerAppSettings.Description)}");

        if (!Version.TryParse(options.ApiVersion, out _))
            failures.Add(
                $"Требуется валидный формат {nameof(SwaggerAppSettings)}:{nameof(SwaggerAppSettings.ApiVersion)}");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
