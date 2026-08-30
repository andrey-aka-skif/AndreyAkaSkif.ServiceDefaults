namespace AndreyAkaSkif.ServiceDefaults.Swagger;

/// <summary>
/// Правило показа Swagger UI
/// </summary>
internal enum SwaggerUiVisibility
{
    /// <summary>
    /// Решает среда: UI показывается везде, кроме Production
    /// </summary>
    ByEnvironment = 0,

    /// <summary>
    /// UI показывается в любой среде
    /// </summary>
    Always = 1,

    /// <summary>
    /// UI не показывается ни в какой среде
    /// </summary>
    Never = 2
}
