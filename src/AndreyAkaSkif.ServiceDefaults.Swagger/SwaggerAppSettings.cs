namespace AndreyAkaSkif.ServiceDefaults.Swagger;

internal sealed record SwaggerAppSettings
{
    /// <summary>
    /// Имя секции конфигурации
    /// </summary>
    public const string SectionName = "Swagger";

    /// <summary>
    /// Адрес спецификации, используемый при отсутствии значения в конфигурации
    /// </summary>
    /// <remarks>
    /// Совпадает с адресом, по которому спецификацию раздаёт <c>MapOpenApi()</c> при имени
    /// документа по умолчанию. Пакет не проверяет, кем спецификация сгенерирована, — адрес
    /// может указывать и на сторонний сервис
    /// </remarks>
    public const string DefaultUrl = "/openapi/v1.json";

    public string Url { get; init; } = DefaultUrl;
    public string Name { get; init; } = string.Empty;
    public SwaggerUiVisibility Visibility { get; init; }

    /// <summary>
    /// Имя документа, показываемое в UI
    /// </summary>
    /// <remarks>
    /// Вычисляется, а не подставляется в PostConfigure: свойство только для чтения,
    /// конвейер параметров его не привязывает и умолчание не затирает
    /// </remarks>
    public string DisplayName => string.IsNullOrWhiteSpace(Name) ? Url : Name;
}
