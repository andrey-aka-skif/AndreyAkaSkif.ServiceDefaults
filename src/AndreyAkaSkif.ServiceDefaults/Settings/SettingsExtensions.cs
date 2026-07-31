using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.Settings;

/// <summary>
/// Методы расширения для регистрации настроек в DI-контейнере.
/// </summary>
public static class SettingsExtensions
{
    /// <summary>
    /// Регистрирует валидированный объект настроек как singleton в DI-контейнере.
    /// </summary>
    /// <typeparam name="T">Тип регистрируемых настроек.</typeparam>
    /// <param name="builder">Построитель приложения.</param>
    /// <returns>Построитель приложения для цепочки вызовов.</returns>
    /// <remarks>
    /// Привязка секции конфигурации и валидация выполняются <strong>в момент вызова</strong>,
    /// а не при первом разрешении сервиса из DI. Некорректная конфигурация обнаруживается
    /// на старте приложения, до вызова <c>Build()</c>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если секция конфигурации отсутствует или содержит некорректные данные.
    /// </exception>
    public static IHostApplicationBuilder AddServiceArgFromValidatedSettingsObject<T>(
        this IHostApplicationBuilder builder)
            where T : class, IValidatableSettingsObject, new()
    {
        var settings = builder.Configuration.CreateValidated<T>();

        builder.Services.AddSingleton(settings);

        return builder;
    }

    /// <summary>
    /// Регистрирует готовый объект как singleton в DI-контейнере.
    /// </summary>
    /// <typeparam name="T">Тип регистрируемого объекта.</typeparam>
    /// <param name="builder">Построитель приложения.</param>
    /// <param name="arg">Экземпляр объекта для регистрации.</param>
    /// <returns>Построитель приложения для цепочки вызовов.</returns>
    public static IHostApplicationBuilder AddServiceArg<T>(
        this IHostApplicationBuilder builder,
        T arg) where T : class
    {
        builder.Services.AddSingleton(arg);

        return builder;
    }
}
