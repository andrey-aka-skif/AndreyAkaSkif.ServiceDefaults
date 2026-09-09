using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.OpenApi;

/// <summary>
/// Разрешает правило доступности в конкретной среде выполнения
/// </summary>
internal static class OpenApiVisibilityExtensions
{
    /// <summary>
    /// Определяет, доступна ли спецификация в среде <paramref name="environment"/>
    /// </summary>
    /// <remarks>
    /// Отсутствующий ключ конфигурации даёт <see cref="OpenApiVisibility.ByEnvironment"/> —
    /// нулевое значение перечисления, поэтому умолчание не требует отдельной подстановки
    /// </remarks>
    public static bool IsVisibleIn(this OpenApiVisibility visibility, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        return visibility switch
        {
            OpenApiVisibility.Always => true,
            OpenApiVisibility.Never => false,
            _ => !environment.IsProduction()
        };
    }
}
