using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace AndreyAkaSkif.ServiceDefaults.Routing;

/// <summary>
/// Методы расширения для сериализации перечислений именами элементов
/// </summary>
public static class StringEnumJsonExtensions
{
    /// <summary>
    /// Сериализовать перечисления именами элементов вместо числовых значений
    /// </summary>
    /// <param name="builder">Строитель приложения</param>
    /// <remarks>
    /// <para>
    /// По умолчанию <c>JsonSerializerDefaults.Web</c> не подключает
    /// <see cref="JsonStringEnumConverter"/>, поэтому перечисления и в телах ответов,
    /// и в спецификации OpenApi выглядят числами. Swagger UI в таком случае предлагает
    /// ввести <c>0</c>, <c>1</c>, <c>2</c> вместо осмысленных имён.
    /// </para>
    /// <para>
    /// Вызывать явно нужно только тогда, когда перечисления встречаются лишь в телах
    /// ответов. Для перечисления в сегменте пути достаточно
    /// <see cref="RouteConstraintExtensions.AddEnumRouteConstraint{TEnum}(IHostApplicationBuilder, string?)"/>:
    /// он выполняет эту настройку сам.
    /// </para>
    /// <para>
    /// Настраиваются оба набора параметров сериализации: <c>Microsoft.AspNetCore.Http.Json</c>
    /// для минимальных API и <c>Microsoft.AspNetCore.Mvc.Json</c> для контроллеров.
    /// Второй нужен ещё и потому, что генератор спецификации Swashbuckle читает именно его:
    /// без этого тела ответов содержали бы имена, а спецификация по-прежнему объявляла бы
    /// перечисление целочисленным. Приложению без контроллеров вторая настройка не вредит.
    /// </para>
    /// <para>
    /// Повторный вызов ничего не меняет: конвертер добавляется, только если его ещё нет.
    /// </para>
    /// </remarks>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    public static IHostApplicationBuilder AddStringEnumJsonSerialization(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.ConfigureHttpJsonOptions(options =>
            AddStringEnumConverter(options.SerializerOptions));

        builder.Services.Configure<MvcJsonOptions>(options =>
            AddStringEnumConverter(options.JsonSerializerOptions));

        return builder;
    }

    private static void AddStringEnumConverter(JsonSerializerOptions options)
    {
        // метод вызывается из AddEnumRouteConstraint для каждого перечисления,
        // поэтому повторный вызов не должен наполнять список копиями конвертера
        if (options.Converters.Any(converter => converter is JsonStringEnumConverter))
            return;

        options.Converters.Add(new JsonStringEnumConverter());
    }
}
