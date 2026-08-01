using AndreyAkaSkif.ServiceDefaults.Settings;

namespace AndreyAkaSkif.ServiceDefaults.Swagger;

internal sealed record SwaggerAppSettings : IValidatableSettingsObject
{
    /// <summary>
    /// Адрес сервера, подставляемый при пустом списке <see cref="Servers"/>.
    /// </summary>
    /// <remarks>
    /// Относительный адрес резолвится Swagger UI от адреса страницы, поэтому спецификация
    /// остаётся работоспособной на любом хосте и за reverse proxy.
    /// </remarks>
    public const string DefaultServer = "/";

    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ApiVersion { get; init; } = string.Empty;
    public List<string> Servers { get; init; } = [];

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException($"Требуется {nameof(SwaggerAppSettings)}:{nameof(Title)}");

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException($"Требуется {nameof(SwaggerAppSettings)}:{nameof(Description)}");

        if (!System.Version.TryParse(ApiVersion, out _))
            throw new ArgumentException($"Требуется валидный формат {nameof(SwaggerAppSettings)}:{nameof(ApiVersion)}");
    }
}
