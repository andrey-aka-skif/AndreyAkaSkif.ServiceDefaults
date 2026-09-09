using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.PostgreSQL;

/// <summary>
/// Предоставляет методы расширения для регистрации контекста Entity Framework Core
/// с провайдером PostgreSQL
/// </summary>
public static class PostgreSQLContextConfigureExtensions
{
    /// <summary>
    /// Регистрирует контекст базы данных с провайдером PostgreSQL
    /// </summary>
    /// <remarks>
    /// <para>
    /// Контекст регистрируется штатным <c>AddDbContext&lt;T&gt;()</c>. Строка подключения
    /// берётся из раздела конфигурации <c>ConnectionStrings</c> по ключу
    /// <paramref name="connectionName"/>
    /// </para>
    /// <para>
    /// Имя строки подключения — конвенция приложения, а не пакета. Умолчание
    /// <c>DefaultConnection</c> совпадает с соглашением шаблонов .NET; приложению, где строки
    /// подключения названы по контексту, достаточно передать своё имя, а не переименовывать
    /// строку подключения в конфигурации и в развёртывании
    /// </para>
    /// <para>
    /// Логирование чувствительных данных
    /// (<see cref="DbContextOptionsBuilder.EnableSensitiveDataLogging"/>) включается только
    /// в среде Development: в остальных средах SQL-запросы с параметрами в логи не попадают
    /// </para>
    /// <para>
    /// Метод только регистрирует контекст. Миграции, <c>EnsureCreated()</c> и проверка
    /// доступности базы данных остаются за приложением
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Тип контекста, производный от <see cref="DbContext"/></typeparam>
    /// <param name="builder">Построитель приложения</param>
    /// <param name="connectionName">
    /// Имя строки подключения в разделе конфигурации <c>ConnectionStrings</c>
    /// </param>
    /// <returns>Тот же экземпляр <paramref name="builder"/> для поддержки цепочки вызовов</returns>
    /// <exception cref="ArgumentException">если <paramref name="connectionName"/> пуст</exception>
    /// <example>
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    ///
    /// builder.AddSimplePostgreSQLContext&lt;AppContext&gt;();
    /// builder.AddSimplePostgreSQLContext&lt;CatalogContext&gt;("Catalog");
    /// </code>
    /// </example>
    public static IHostApplicationBuilder AddSimplePostgreSQLContext<T>(
        this IHostApplicationBuilder builder,
        string connectionName = "DefaultConnection") where T : DbContext
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionName);

        builder.Services.AddDbContext<T>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString(connectionName));
            options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
        });

        return builder;
    }
}
