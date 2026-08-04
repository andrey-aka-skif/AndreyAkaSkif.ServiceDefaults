using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Settings;

/// <summary>
/// Методы расширения для регистрации настроек в DI-контейнере.
/// </summary>
public static class SettingsExtensions
{
    /// <summary>
    /// Зарегистрировать настройки в конвейере параметров с обязательной валидацией
    /// </summary>
    /// <typeparam name="T">Тип настроек</typeparam>
    /// <typeparam name="TValidator">
    /// Тип правил валидации. Активируется контейнером, поэтому может принимать
    /// зависимости через конструктор
    /// </typeparam>
    /// <param name="builder">Построитель приложения</param>
    /// <param name="sectionName">
    /// Имя секции конфигурации. По умолчанию — имя типа настроек: для <c>CorsPolicy</c>
    /// это <c>"CorsPolicy"</c>
    /// </param>
    /// <remarks>
    /// <para>
    /// Настройки доступны из DI-контейнера как <see cref="IOptions{T}"/>. Это вариант
    /// для инфраструктуры: адаптеров, промежуточного ПО и прочего кода, которому
    /// знакомство с конвейером параметров ничего не стоит. Доменному коду настройки
    /// отдаются развёрнутым значением — см. <see cref="AddAppSettings{T, TValidator}"/>.
    /// </para>
    /// <para>
    /// Валидатор — обязательный параметр типа, а не перегрузка: настройки без правил
    /// валидации зарегистрировать нельзя.
    /// </para>
    /// <para>
    /// Некорректная конфигурация роняет приложение при старте хоста — <c>ValidateOnStart</c>, —
    /// а не при первом разрешении настроек из DI. Это позже, чем <c>Build()</c>, но всё ещё
    /// до первого запроса.
    /// </para>
    /// <para>
    /// Повторный вызов с той же парой типов ничего не меняет: правила валидации
    /// регистрируются один раз.
    /// </para>
    /// </remarks>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    /// <exception cref="ArgumentException">
    /// если <paramref name="sectionName"/> задан и пуст
    /// </exception>
    public static IHostApplicationBuilder AddValidatedOptions<T, TValidator>(
        this IHostApplicationBuilder builder,
        string? sectionName = null)
        where T : class
        where TValidator : class, IValidateOptions<T>
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (sectionName is not null && string.IsNullOrWhiteSpace(sectionName))
            throw new ArgumentException("Не задано имя секции конфигурации", nameof(sectionName));

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IValidateOptions<T>, TValidator>());

        builder.Services
            .AddOptions<T>()
            .BindConfiguration(sectionName ?? typeof(T).Name)
            .ValidateOnStart();

        return builder;
    }

    /// <summary>
    /// Зарегистрировать настройки приложения как значение, с обязательной валидацией
    /// </summary>
    /// <typeparam name="T">Тип настроек</typeparam>
    /// <typeparam name="TValidator">
    /// Тип правил валидации. Активируется контейнером, поэтому может принимать
    /// зависимости через конструктор
    /// </typeparam>
    /// <param name="builder">Построитель приложения</param>
    /// <param name="sectionName">
    /// Имя секции конфигурации. По умолчанию — имя типа настроек: для <c>DemoAppSettings</c>
    /// это <c>"DemoAppSettings"</c>
    /// </param>
    /// <remarks>
    /// <para>
    /// Отличие от <see cref="AddValidatedOptions{T, TValidator}"/> — в том, что попадает
    /// в DI-контейнер. Здесь дополнительно регистрируется само значение настроек, поэтому
    /// потребитель объявляет зависимость как <c>T</c>, а не как <c>IOptions&lt;T&gt;</c>:
    /// <code>
    /// public sealed record DemoAppSettings
    /// {
    ///     public string Greeting { get; init; } = string.Empty;
    /// }
    ///
    /// internal sealed class DemoAppSettingsValidator : IValidateOptions&lt;DemoAppSettings&gt;
    /// {
    ///     public ValidateOptionsResult Validate(string? name, DemoAppSettings options)
    ///         =&gt; string.IsNullOrWhiteSpace(options.Greeting)
    ///             ? ValidateOptionsResult.Fail("Требуется DemoAppSettings:Greeting")
    ///             : ValidateOptionsResult.Success;
    /// }
    ///
    /// builder.AddAppSettings&lt;DemoAppSettings, DemoAppSettingsValidator&gt;();
    ///
    /// internal sealed class GreetingService(DemoAppSettings settings);
    /// </code>
    /// Класс настроек остаётся чистым POCO: ни интерфейсов, ни атрибутов, ни ссылки
    /// на этот пакет. Доменный код о конвейере параметров не знает.
    /// </para>
    /// <para>
    /// Значение читается один раз, при первом разрешении из контейнера. Изменение
    /// конфигурации на лету на него не влияет — за перечитыванием следует обращаться
    /// к <see cref="IOptionsMonitor{T}"/>.
    /// </para>
    /// </remarks>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    /// <exception cref="ArgumentException">
    /// если <paramref name="sectionName"/> задан и пуст
    /// </exception>
    public static IHostApplicationBuilder AddAppSettings<T, TValidator>(
        this IHostApplicationBuilder builder,
        string? sectionName = null)
        where T : class
        where TValidator : class, IValidateOptions<T>
    {
        builder.AddValidatedOptions<T, TValidator>(sectionName);

        builder.Services.TryAddSingleton(sp => sp.GetRequiredService<IOptions<T>>().Value);

        return builder;
    }
}
