using AndreyAkaSkif.ServiceDefaults.Routing;
using AndreyAkaSkif.ServiceDefaults.Samples.Api.Endpoints;

namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.AppConfiguration;

/// <summary>
/// Ограничения параметров маршрута, специфичные для приложения
/// </summary>
internal static class AppRouteConstraintsConfigureExtensions
{
    /// <summary>
    /// Зарегистрировать ограничения параметров маршрута
    /// </summary>
    /// <remarks>
    /// <para>
    /// Сюда добавляются ограничения для собственных типов приложения: перечисления
    /// через <c>AddEnumRouteConstraint</c> и любые свои реализации <c>IRouteConstraint</c>
    /// через <c>AddRouteConstraint</c>.
    /// </para>
    /// <para>
    /// Ограничение <c>DemoChannel</c> доступно в шаблоне маршрута как "demoChannel" —
    /// имя по умолчанию берётся от имени типа. Заодно включается сериализация
    /// перечислений именами, иначе в теле ответа и в спецификации они выглядели бы
    /// как 0, 1, 2.
    /// </para>
    /// </remarks>
    public static IHostApplicationBuilder AddAppRouteConstraints(this IHostApplicationBuilder builder)
    {
        builder.AddEnumRouteConstraint<DemoChannel>();

        return builder;
    }
}
