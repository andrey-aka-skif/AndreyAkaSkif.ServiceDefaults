namespace AndreyAkaSkif.ServiceDefaults.OpenApi;

internal sealed record OpenApiAppSettings
{
    /// <summary>
    /// Имя секции конфигурации
    /// </summary>
    public const string SectionName = "OpenApi";

    /// <summary>
    /// Имя документа, используемое при отсутствии значения в конфигурации
    /// </summary>
    /// <remarks>
    /// Имя попадает в адрес спецификации, поэтому не связано ни с версией контракта,
    /// ни с версией сборки: адрес, по которому генерируются клиенты, меняться не должен
    /// </remarks>
    public const string DefaultDocumentName = "v1";

    /// <summary>
    /// Версия контракта API, используемая при отсутствии значения в конфигурации
    /// </summary>
    public const string DefaultVersion = "1.0";

    /// <summary>
    /// Адрес сервера, подставляемый при пустом списке <see cref="Servers"/>
    /// </summary>
    /// <remarks>
    /// Относительный адрес резолвится от адреса страницы, поэтому спецификация остаётся
    /// работоспособной на любом хосте и за reverse proxy
    /// </remarks>
    public const string DefaultServer = "/";

    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string DocumentName { get; init; } = DefaultDocumentName;
    public string Version { get; init; } = DefaultVersion;
    public List<string> Servers { get; init; } = [];
    public OpenApiVisibility Visibility { get; init; }
}
