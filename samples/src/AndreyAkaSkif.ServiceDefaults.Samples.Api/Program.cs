using AndreyAkaSkif.ServiceDefaults.Cors;
using AndreyAkaSkif.ServiceDefaults.ErrorHandling;
using AndreyAkaSkif.ServiceDefaults.HealthChecking;
using AndreyAkaSkif.ServiceDefaults.Routing;
using AndreyAkaSkif.ServiceDefaults.Samples.Api.Endpoints;
using AndreyAkaSkif.ServiceDefaults.Samples.Api.Settings;
using AndreyAkaSkif.ServiceDefaults.Serilog;
using AndreyAkaSkif.ServiceDefaults.Settings;
using AndreyAkaSkif.ServiceDefaults.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Логирование через Serilog. Конфигурация — секция "Serilog".
// Пакет добавляет Serilog к уже настроенным провайдерам, поэтому провайдеры
// по умолчанию убираются явно — иначе каждая запись попадёт в консоль дважды
builder.Logging.ClearProviders();
builder.AddConfiguredLoggingViaSerilog();

// Настройки приложения: секция "DemoAppSettings" валидируется здесь же,
// до Build(), и регистрируется в DI
builder.AddServiceArgFromValidatedSettingsObject<DemoAppSettings>();

// ProblemDetails. В Development в ответ добавляется поле "exception"
builder.AddExtendedErrorHandling();

// OpenApi через Swagger. Конфигурация — секция "SwaggerAppSettings".
// SwaggerGen строит спецификацию поверх ApiExplorer, а пакет его не регистрирует,
// поэтому для minimal API вызов AddEndpointsApiExplorer() обязателен
builder.Services.AddEndpointsApiExplorer();
builder.AddConfiguredOpenApiViaSwagger();

// Политика CORS. Конфигурация — секция "CorsPolicy"
builder.AddConfiguredCorsPolicy();

// Конечная точка /health и её отображение в Swagger UI
builder.AddHealthCheckEndpointWithSwagger();

// --- OpenApi без Swagger -----------------------------------------------------
// Альтернатива паре AddConfiguredOpenApiViaSwagger/UseConfiguredOpenApiViaSwagger:
// встроенная в ASP.NET генерация спецификации без Swagger UI.
// Чтобы включить — раскомментировать ProjectReference на
// AndreyAkaSkif.ServiceDefaults.OpenApi в csproj, using
// AndreyAkaSkif.ServiceDefaults.OpenApi и убрать вызовы Swagger:
//
// builder.AddDefaultOpenApi();
// -----------------------------------------------------------------------------

// --- PostgreSQL --------------------------------------------------------------
// Чтобы включить — раскомментировать ProjectReference на
// AndreyAkaSkif.ServiceDefaults.PostgreSQL в csproj, секцию "ConnectionStrings"
// в appsettings.Development.json, using-и Microsoft.EntityFrameworkCore и
// AndreyAkaSkif.ServiceDefaults.PostgreSQL, а также класс DemoDbContext
// в конце файла. Требуется запущенный PostgreSQL:
//
// builder.AddSimplePostgreSQLContext<DemoDbContext>();
// -----------------------------------------------------------------------------

var app = builder.Build();

app.MapDemoEndpoints();

app.UseErrorHandling();
app.UseConfiguredOpenApiViaSwagger();
app.UseConfiguredCorsPolicy();
app.UseConfiguredPathBase();
app.MapHealthCheckEndpoint();

// Парная часть блока "OpenApi без Swagger":
//
// app.UseDefaultOpenApi();

app.Run();

// Парная часть блока "PostgreSQL". Строка подключения читается из
// ConnectionStrings:DefaultConnection — имя фиксировано в пакете:
//
// internal sealed class DemoDbContext(DbContextOptions<DemoDbContext> options) : DbContext(options)
// {
// }
