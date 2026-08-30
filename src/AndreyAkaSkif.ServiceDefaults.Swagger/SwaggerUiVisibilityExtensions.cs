using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.Swagger;

/// <summary>
/// Разрешение правила показа в конкретной среде выполнения
/// </summary>
internal static class SwaggerUiVisibilityExtensions
{
    /// <summary>
    /// Показывается ли UI в среде <paramref name="environment"/>
    /// </summary>
    /// <remarks>
    /// Отсутствующий ключ конфигурации даёт <see cref="SwaggerUiVisibility.ByEnvironment"/> —
    /// нулевое значение перечисления, поэтому умолчание не требует отдельной подстановки
    /// </remarks>
    public static bool IsVisibleIn(this SwaggerUiVisibility visibility, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        return visibility switch
        {
            SwaggerUiVisibility.Always => true,
            SwaggerUiVisibility.Never => false,
            _ => !environment.IsProduction()
        };
    }
}
