using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.Routing;

/// <summary>
/// Методы расширения для регистрации ограничений параметров маршрута в DI-контейнере
/// </summary>
public static class RouteConstraintExtensions
{
    /// <summary>
    /// Зарегистрировать произвольное ограничение параметра маршрута под указанным именем
    /// </summary>
    /// <typeparam name="TConstraint">Тип ограничения</typeparam>
    /// <param name="builder">Строитель приложения</param>
    /// <param name="name">Имя, под которым ограничение доступно в шаблоне маршрута</param>
    /// <remarks>
    /// <para>
    /// Метод общего назначения: он не решает конкретную задачу, а служит основой для
    /// построения ограничений. Для перечислений готовое решение уже есть —
    /// <see cref="AddEnumRouteConstraint{TEnum}(IHostApplicationBuilder, string?)"/>,
    /// который построен поверх этого метода. Напрямую метод нужен, когда написана
    /// собственная реализация <c>IRouteConstraint</c>.
    /// </para>
    /// <para>
    /// Пример собственного ограничения — только чётные числа:
    /// <code>
    /// public sealed class EvenNumberRouteConstraint : IRouteConstraint
    /// {
    ///     public bool Match(
    ///         HttpContext? httpContext,
    ///         IRouter? route,
    ///         string routeKey,
    ///         RouteValueDictionary values,
    ///         RouteDirection routeDirection)
    ///         => values.TryGetValue(routeKey, out var value)
    ///            &amp;&amp; int.TryParse(value?.ToString(), out var number)
    ///            &amp;&amp; number % 2 == 0;
    /// }
    ///
    /// builder.AddRouteConstraint&lt;EvenNumberRouteConstraint&gt;("even");
    ///
    /// app.MapGet("items/{id:even}", (int id) => id);
    /// </code>
    /// </para>
    /// <para>
    /// Имя используется в шаблоне после двоеточия: при регистрации под именем
    /// <c>"channelType"</c> шаблон выглядит как <c>"channel/{channel:channelType}"</c>.
    /// </para>
    /// <para>
    /// Повторная регистрация того же типа под тем же именем допустима и ничего не меняет.
    /// Регистрация другого типа под занятым именем считается ошибкой: иначе конфликт
    /// проявился бы при разрешении <c>RouteOptions</c>, вдалеке от места регистрации.
    /// </para>
    /// </remarks>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    /// <exception cref="ArgumentException">если <paramref name="name"/> пуст</exception>
    /// <exception cref="InvalidOperationException">
    /// если под этим именем уже зарегистрировано ограничение другого типа
    /// </exception>
    public static IHostApplicationBuilder AddRouteConstraint<TConstraint>(
        this IHostApplicationBuilder builder,
        string name)
        where TConstraint : IRouteConstraint
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Не задано имя ограничения маршрута", nameof(name));

        builder.Services.Configure<RouteOptions>(options =>
        {
            if (options.ConstraintMap.TryGetValue(name, out var registered))
            {
                if (registered == typeof(TConstraint))
                    return;

                throw new InvalidOperationException(
                    $"Имя ограничения маршрута \"{name}\" уже занято типом {registered.Name}. " +
                    $"Задайте другое имя для {typeof(TConstraint).Name}.");
            }

            options.ConstraintMap[name] = typeof(TConstraint);
        });

        return builder;
    }

    /// <summary>
    /// Зарегистрировать ограничение параметра маршрута значениями перечисления
    /// </summary>
    /// <typeparam name="TEnum">Тип перечисления, допустимый в сегменте пути</typeparam>
    /// <param name="builder">Строитель приложения</param>
    /// <param name="name">
    /// Имя ограничения в шаблоне маршрута. По умолчанию — имя типа перечисления
    /// со строчной первой буквой: для <c>ChannelType</c> это <c>"channelType"</c>
    /// </param>
    /// <remarks>
    /// <para>
    /// Пример использования:
    /// <code>
    /// builder.AddEnumRouteConstraint&lt;ChannelType&gt;();
    ///
    /// app.MapGet("channel/{channel:channelType}", (ChannelType channel) => channel);
    /// </code>
    /// </para>
    /// <para>
    /// Сопоставление регистронезависимо, числовые значения также допустимы, а значение
    /// приводится к каноническому имени элемента перечисления до привязки аргумента.
    /// Значение вне перечисления маршрут не выбирает — ответ 404.
    /// </para>
    /// <para>
    /// Работает одинаково для минимальных API и для контроллеров: <c>ConstraintMap</c>
    /// принадлежит <c>RouteOptions</c>, который читает общий для обоих стилей резолвер
    /// ограничений.
    /// </para>
    /// <para>
    /// Дополнительно включается сериализация перечислений именами
    /// (<see cref="StringEnumJsonExtensions.AddStringEnumJsonSerialization(IHostApplicationBuilder)"/>):
    /// без неё спецификация OpenApi объявляла бы перечисление целочисленным, и задача
    /// читаемости решалась бы наполовину. Настройка идемпотентна, поэтому регистрация
    /// нескольких перечислений не приводит к её дублированию.
    /// </para>
    /// </remarks>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    /// <exception cref="InvalidOperationException">
    /// если под этим именем уже зарегистрировано ограничение другого типа
    /// </exception>
    public static IHostApplicationBuilder AddEnumRouteConstraint<TEnum>(
        this IHostApplicationBuilder builder,
        string? name = null)
        where TEnum : struct, Enum
        => builder
            .AddRouteConstraint<EnumRouteConstraint<TEnum>>(name ?? DefaultNameFor<TEnum>())
            .AddStringEnumJsonSerialization();

    private static string DefaultNameFor<TEnum>()
        where TEnum : struct, Enum
    {
        var typeName = typeof(TEnum).Name;

        return char.ToLowerInvariant(typeName[0]) + typeName[1..];
    }
}
