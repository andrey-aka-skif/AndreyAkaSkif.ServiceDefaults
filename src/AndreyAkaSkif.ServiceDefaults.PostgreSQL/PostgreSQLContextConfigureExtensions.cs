using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.PostgreSQL;

/// <summary>
/// Предоставляет методы расширения для добавления простого контекста PostgreSQL
/// </summary>
public static class PostgreSQLContextConfigureExtensions
{
    /// <summary>
    /// Регистрирует контекст базы данных Entity Framework Core с PostgreSQL провайдером.
    /// </summary>
    /// <typeparam name="T">Тип контекста базы данных, производный от <see cref="DbContext"/>.</typeparam>
    /// <param name="builder">Построитель приложения для добавления сервисов.</param>
    /// <returns>Построитель приложения для цепочки вызовов.</returns>
    /// <remarks>
    /// <para>
    /// <strong>Важно:</strong> Строка подключения должна быть определена в конфигурации приложения 
    /// в разделе <c>"ConnectionStrings"</c> с именем <c>"DefaultConnection"</c>. 
    /// Пример конфигурации в appsettings.json:
    /// <code>
    /// {
    ///   "ConnectionStrings": {
    ///     "DefaultConnection": "Host=localhost;Database=mydb;Username=user;Password=pass"
    ///   }
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Логирование чувствительных данных (<see cref="DbContextOptionsBuilder.EnableSensitiveDataLogging"/>) автоматически 
    /// включается только в Development среде. 
    /// В других средах логирование SQL-запросов с параметрами отключено в целях безопасности.
    /// </para>
    /// <example>
    /// Пример использования в Program.cs:
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// builder.AddSimplePostgreSQLContext&lt;MyDbContext&gt;();
    /// </code>
    /// </example>
    /// </remarks>
    public static IHostApplicationBuilder AddSimplePostgreSQLContext<T>(this IHostApplicationBuilder builder) where T : DbContext
    {
        builder.Services.AddDbContext<T>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
        });

        return builder;
    }
}
