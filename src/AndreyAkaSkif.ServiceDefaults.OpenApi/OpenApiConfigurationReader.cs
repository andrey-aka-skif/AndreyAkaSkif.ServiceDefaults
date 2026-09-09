using Microsoft.Extensions.Configuration;

namespace AndreyAkaSkif.ServiceDefaults.OpenApi;

/// <summary>
/// Читает значения секции <c>OpenApi</c>, которые нужны раньше конвейера параметров
/// </summary>
internal static class OpenApiConfigurationReader
{
    /// <summary>
    /// Имя документа
    /// </summary>
    /// <remarks>
    /// Читается из конфигурации напрямую: <c>AddOpenApi(documentName)</c> требует имя
    /// в момент регистрации, до того как настройки будут привязаны и провалидированы.
    /// Остальная атрибуция берётся из <see cref="OpenApiAppSettings"/> лениво
    /// </remarks>
    public static string ReadDocumentName(IConfiguration configuration)
    {
        var documentName = configuration[
            $"{OpenApiAppSettings.SectionName}:{nameof(OpenApiAppSettings.DocumentName)}"];

        return string.IsNullOrWhiteSpace(documentName)
            ? OpenApiAppSettings.DefaultDocumentName
            : documentName;
    }

    /// <summary>
    /// Правило доступности спецификации
    /// </summary>
    /// <remarks>
    /// Нужно паре методов без конфигурации, где настройки в конвейер параметров
    /// не привязываются вовсе
    /// </remarks>
    public static OpenApiVisibility ReadVisibility(IConfiguration configuration)
        => configuration.GetValue<OpenApiVisibility>(
            $"{OpenApiAppSettings.SectionName}:{nameof(OpenApiAppSettings.Visibility)}");
}
