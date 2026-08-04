using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AndreyAkaSkif.ServiceDefaults.Routing;

/// <summary>
/// Ограничение параметра маршрута значениями перечисления <typeparamref name="TEnum"/>
/// </summary>
/// <typeparam name="TEnum">Тип перечисления, допустимый в сегменте пути</typeparam>
/// <remarks>
/// <para>
/// Регистрируется через
/// <see cref="RouteConstraintExtensions.AddEnumRouteConstraint{TEnum}(Microsoft.Extensions.Hosting.IHostApplicationBuilder, string?)"/>
/// и применяется в шаблоне маршрута: <c>"channel/{channel:channelType}"</c>.
/// </para>
/// <para>
/// Сопоставление регистронезависимо, числовые значения также допустимы. Значение,
/// не соответствующее ни одному элементу перечисления, маршрут не выбирает — клиент
/// получает 404, а не 400 при привязке аргумента.
/// </para>
/// </remarks>
public sealed class EnumRouteConstraint<TEnum> : IRouteConstraint
    where TEnum : struct, Enum
{
    /// <summary>
    /// Проверяет значение параметра маршрута и приводит его к каноническому виду
    /// </summary>
    /// <param name="httpContext">Контекст запроса; ограничением не используется</param>
    /// <param name="route">Маршрутизатор; ограничением не используется</param>
    /// <param name="routeKey">Имя проверяемого параметра маршрута</param>
    /// <param name="values">Значения параметров маршрута</param>
    /// <param name="routeDirection">
    /// Направление сопоставления. Приведение к каноническому имени выполняется только
    /// для входящего запроса: при генерации URL значение принадлежит вызывающему коду
    /// </param>
    /// <returns>
    /// <see langword="true"/>, если значение соответствует элементу <typeparamref name="TEnum"/>
    /// </returns>
    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (!values.TryGetValue(routeKey, out var rawValue))
            return false;

        var text = Convert.ToString(rawValue, CultureInfo.InvariantCulture);

        if (string.IsNullOrEmpty(text))
            return false;

        // Enum.TryParse принимает и число вне диапазона: "999" разберётся в (TEnum)999,
        // поэтому результат дополнительно проверяется через IsDefined
        if (!Enum.TryParse<TEnum>(text, ignoreCase: true, out var parsed) || !Enum.IsDefined(parsed))
            return false;

        // Привязка аргументов в минимальных API регистрозависима: она использует
        // Enum.TryParse<T>(string, out T) без ignoreCase. Без приведения к каноническому
        // имени "/channel/currenta" прошёл бы маршрутизацию и упал бы с 400.
        // При генерации URL значение принадлежит вызывающему коду и не изменяется.
        if (routeDirection == RouteDirection.IncomingRequest)
            values[routeKey] = parsed.ToString();

        return true;
    }
}
