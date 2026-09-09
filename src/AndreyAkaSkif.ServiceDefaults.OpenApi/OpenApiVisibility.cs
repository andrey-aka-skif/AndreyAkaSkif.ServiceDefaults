namespace AndreyAkaSkif.ServiceDefaults.OpenApi;

/// <summary>
/// Определяет правило доступности спецификации OpenAPI
/// </summary>
internal enum OpenApiVisibility
{
    /// <summary>
    /// Решает среда: спецификация доступна везде, кроме Production
    /// </summary>
    ByEnvironment = 0,

    /// <summary>
    /// Спецификация доступна в любой среде
    /// </summary>
    Always = 1,

    /// <summary>
    /// Спецификация недоступна ни в какой среде
    /// </summary>
    Never = 2
}
