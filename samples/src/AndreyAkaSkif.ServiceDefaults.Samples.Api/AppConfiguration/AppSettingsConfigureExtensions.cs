using AndreyAkaSkif.ServiceDefaults.Samples.Api.AppSettings;
using AndreyAkaSkif.ServiceDefaults.Settings;

namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.AppConfiguration;

/// <summary>
/// Настройки приложения
/// </summary>
internal static class AppSettingsConfigureExtensions
{
    /// <summary>
    /// Зарегистрировать объекты настроек приложения
    /// </summary>
    /// <remarks>
    /// <para>
    /// Сюда добавляются все собственные типы настроек приложения. Каждый читается
    /// из одноимённой секции конфигурации, валидируется в момент вызова — то есть
    /// до <c>Build()</c> — и попадает в DI-контейнер готовым экземпляром.
    /// </para>
    /// <para>
    /// Это типы границы приложения; они лежат в каталоге <c>AppSettings</c>.
    /// Объекты-параметры сервисов сюда не относятся — их место рядом с сервисом,
    /// см. <c>AppServicesConfigureExtensions</c>.
    /// </para>
    /// </remarks>
    public static IHostApplicationBuilder AddAppSettings(this IHostApplicationBuilder builder)
    {
        builder.AddServiceArgFromValidatedSettingsObject<DemoAppSettings>();

        return builder;
    }
}
