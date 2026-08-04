using AndreyAkaSkif.ServiceDefaults.Samples.Api.AppSettings;
using AndreyAkaSkif.ServiceDefaults.Samples.Api.Services;

namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.AppConfiguration;

/// <summary>
/// Сервисы приложения
/// </summary>
internal static class AppServicesConfigureExtensions
{
    /// <summary>
    /// Зарегистрировать сервисы приложения
    /// </summary>
    /// <remarks>
    /// <para>
    /// Сюда складываются регистрации репозиториев, сервисов домена, валидаторов
    /// и прочих зависимостей приложения.
    /// </para>
    /// <para>
    /// Здесь же граничные настройки отображаются в аргументы домена:
    /// <see cref="DemoAppSettings"/> знает про секцию конфигурации,
    /// <see cref="GreetingServiceArgs"/> — нет, и переход между ними происходит
    /// в composition root, а не внутри сервиса. Если аргумент не зависит от других
    /// сервисов, его можно собрать сразу и зарегистрировать короче:
    /// <c>builder.AddServiceArg(new GreetingServiceArgs("Привет"))</c>.
    /// </para>
    /// <para>
    /// Когда регистраций становится много, этот файл делится по темам —
    /// рядом появляются соседи вроде <c>AppValidatorsConfigureExtensions</c>,
    /// а <c>Program.cs</c> при этом не растёт.
    /// </para>
    /// </remarks>
    public static IHostApplicationBuilder AddAppServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton(sp =>
            new GreetingServiceArgs(sp.GetRequiredService<DemoAppSettings>().Greeting));

        builder.Services.AddSingleton<IGreetingService, GreetingService>();

        return builder;
    }
}
