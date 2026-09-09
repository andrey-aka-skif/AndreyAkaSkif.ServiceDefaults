using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyAkaSkif.ServiceDefaults.Tests;

public class PostgreSQLContextConfigureExtensionsTests
{
    private const string DefaultConnectionString = "Host=default-host;Database=db";
    private const string CatalogConnectionString = "Host=catalog-host;Database=catalog";

    private static readonly Dictionary<string, string?> Connections = new()
    {
        ["ConnectionStrings:DefaultConnection"] = DefaultConnectionString,
        ["ConnectionStrings:Catalog"] = CatalogConnectionString,
    };

    [Fact]
    public void AddSimplePostgreSQLContext_ShouldUseDefaultConnection_WhenNameOmitted()
    {
        // Arrange
        var builder = CreateBuilderWith(Connections);

        // Act
        builder.AddSimplePostgreSQLContext<TestDbContext>();
        using var host = builder.Build();

        // Assert
        Assert.Equal(DefaultConnectionString, ConnectionStringOf(host));
    }

    [Fact]
    public void AddSimplePostgreSQLContext_ShouldUseNamedConnection_WhenNameGiven()
    {
        // Arrange
        var builder = CreateBuilderWith(Connections);

        // Act
        builder.AddSimplePostgreSQLContext<TestDbContext>("Catalog");
        using var host = builder.Build();

        // Assert
        Assert.Equal(CatalogConnectionString, ConnectionStringOf(host));
    }

    private static HostApplicationBuilder CreateBuilderWith(
        Dictionary<string, string?> configuration)
    {
        var builder = Host.CreateEmptyApplicationBuilder(null);
        builder.Configuration.AddInMemoryCollection(configuration);

        return builder;
    }

    // строку подключения, с которой контекст в итоге зарегистрирован, отдаёт публичный API
    // EF Core: соединение при этом не открывается, живая база данных не нужна
    private static string? ConnectionStringOf(IHost host)
    {
        var options = host.Services.GetRequiredService<DbContextOptions<TestDbContext>>();

        return RelationalOptionsExtension.Extract(options).ConnectionString;
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options)
        : DbContext(options);
}
