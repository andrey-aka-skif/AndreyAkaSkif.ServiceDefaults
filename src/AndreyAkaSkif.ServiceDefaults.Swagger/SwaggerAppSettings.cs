using Microsoft.Extensions.Configuration;

namespace AndreyAkaSkif.ServiceDefaults.Swagger;

internal sealed record SwaggerAppSettings
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ApiVersion { get; init; } = string.Empty;
    public List<string> Servers { get; init; } = [];

    private SwaggerAppSettings() { }

    public static SwaggerAppSettings CreateValidated(IConfiguration configuration)
    {
        var settings = new SwaggerAppSettings();
        configuration.GetSection(nameof(SwaggerAppSettings)).Bind(settings);
        settings.Validate();
        return settings;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException($"Требуется {nameof(SwaggerAppSettings)}:{nameof(Title)}");

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException($"Требуется {nameof(SwaggerAppSettings)}:{nameof(Description)}");

        if (!System.Version.TryParse(ApiVersion, out _))
            throw new ArgumentException($"Требуется валидный формат {nameof(SwaggerAppSettings)}:{nameof(ApiVersion)}");

        if (Servers == null || Servers.Count == 0)
            throw new ArgumentException($"Требуется хотя бы один сервер в {nameof(SwaggerAppSettings)}:{nameof(Servers)}");
    }
}
