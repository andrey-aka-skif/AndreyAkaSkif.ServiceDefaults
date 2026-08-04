using AndreyAkaSkif.ServiceDefaults.Samples.Api.AppSettings;
using AndreyAkaSkif.ServiceDefaults.Settings;

namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.AppConfiguration;

/// <summary>
/// Настройки приложения
/// </summary>
internal static class AppSettingsConfigureExtensions
{
    /// <summary>
    /// Зарегистрировать все объекты настроек приложения
    /// </summary>
    /// <remarks>
    /// <para>
    /// Сюда добавляются все собственные типы настроек приложения. Каждый читается
    /// из одноимённой секции конфигурации, валидируется на старте хоста — то есть
    /// до первого запроса — и попадает в DI-контейнер готовым значением, а не
    /// как <c>IOptions&lt;T&gt;</c>: этим <c>AddAppSettings&lt;T, TValidator&gt;()</c>
    /// и отличается от <c>AddValidatedOptions&lt;T, TValidator&gt;()</c>.
    /// </para>
    /// <para>
    /// Это типы границы приложения; они лежат в каталоге <c>AppSettings</c>.
    /// Объекты-параметры сервисов сюда не относятся — их место рядом с сервисом,
    /// см. <c>AppServicesConfigureExtensions</c>.
    /// </para>
    /// </remarks>
    public static IHostApplicationBuilder AddAllAppSettings(this IHostApplicationBuilder builder)
    {
        builder.AddAppSettings<DemoAppSettings, DemoAppSettingsValidator>();

        return builder;
    }
}
