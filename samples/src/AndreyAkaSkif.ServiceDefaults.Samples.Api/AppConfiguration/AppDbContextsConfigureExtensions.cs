namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.AppConfiguration;

/// <summary>
/// Контексты базы данных
/// </summary>
internal static class AppDbContextsConfigureExtensions
{
    /// <summary>
    /// Зарегистрировать контексты базы данных
    /// </summary>
    /// <remarks>
    /// <para>
    /// В образце контекст выключен: он требует запущенного PostgreSQL, а образец должен
    /// запускаться одной командой. Пакет
    /// <c>AndreyAkaSkif.ServiceDefaults.PostgreSQL</c> подключает контекст EF Core
    /// на Npgsql одним вызовом, строка подключения читается из
    /// <c>ConnectionStrings:DefaultConnection</c> — имя фиксировано в пакете.
    /// </para>
    /// <para>
    /// Чтобы включить, нужно раскомментировать: <c>ProjectReference</c>
    /// на <c>AndreyAkaSkif.ServiceDefaults.PostgreSQL</c> в csproj, секцию
    /// <c>ConnectionStrings</c> в appsettings.Development.json, а в этом файле —
    /// <c>using</c>-и, тело метода и класс <c>DemoDbContext</c>.
    /// </para>
    /// </remarks>
    public static IHostApplicationBuilder AddAppDbContexts(this IHostApplicationBuilder builder)
    {
        // builder.AddSimplePostgreSQLContext<DemoDbContext>();

        return builder;
    }
}

// Парная часть блока PostgreSQL. Требует using-ов Microsoft.EntityFrameworkCore
// и AndreyAkaSkif.ServiceDefaults.PostgreSQL:
//
// internal sealed class DemoDbContext(DbContextOptions<DemoDbContext> options) : DbContext(options)
// {
// }
