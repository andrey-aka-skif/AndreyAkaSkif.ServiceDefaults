using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.OpenApi;

internal sealed class OpenApiAppSettingsValidator : IValidateOptions<OpenApiAppSettings>
{
    public ValidateOptionsResult Validate(string? name, OpenApiAppSettings options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Title))
            failures.Add(
                $"Требуется {OpenApiAppSettings.SectionName}:{nameof(OpenApiAppSettings.Title)}");

        // имя документа — сегмент адреса спецификации, поэтому разделитель пути в нём
        // ломает маршрут, а не просто выглядит странно
        if (string.IsNullOrWhiteSpace(options.DocumentName))
            failures.Add(
                $"Требуется {OpenApiAppSettings.SectionName}:{nameof(OpenApiAppSettings.DocumentName)}");
        else if (options.DocumentName.Contains('/', StringComparison.Ordinal))
            failures.Add(
                $"{OpenApiAppSettings.SectionName}:{nameof(OpenApiAppSettings.DocumentName)} "
                + "не может содержать \"/\"");

        // версия контракта по спецификации OpenAPI — произвольная строка,
        // на числовые части не разбирается
        if (string.IsNullOrWhiteSpace(options.Version))
            failures.Add(
                $"Требуется {OpenApiAppSettings.SectionName}:{nameof(OpenApiAppSettings.Version)}");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
