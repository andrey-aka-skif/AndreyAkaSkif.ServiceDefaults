using AndreyAkaSkif.ServiceDefaults.Cors;
using AndreyAkaSkif.ServiceDefaults.ErrorHandling;
using AndreyAkaSkif.ServiceDefaults.HealthChecking;
using AndreyAkaSkif.ServiceDefaults.Routing;
using AndreyAkaSkif.ServiceDefaults.Samples.Api.AppConfiguration;
using AndreyAkaSkif.ServiceDefaults.Samples.Api.Endpoints;
using AndreyAkaSkif.ServiceDefaults.Serilog;
using AndreyAkaSkif.ServiceDefaults.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Логирование через Serilog. Конфигурация — секция "Serilog".
// По умолчанию Serilog ставится монопольно: провайдеры по умолчанию не пишут
builder.AddConfiguredLoggingViaSerilog();

// Конфигурация, специфичная для приложения. Подробности — в каталоге AppConfiguration
builder.AddAppSettings();
builder.AddAppDbContexts();
builder.AddAppServices();
builder.AddAppHttpClients();
builder.AddAppRouteConstraints();

// ProblemDetails. В Development в ответ добавляется поле "exception"
builder.AddExtendedErrorHandling();

// OpenApi через Swagger. Конфигурация — секция "SwaggerAppSettings"
builder.AddConfiguredOpenApiViaSwagger();

// Политика CORS. Конфигурация — секция "CorsPolicy"
builder.AddConfiguredCorsPolicy();

// Базовый путь. Конфигурация — секция "PathBaseAppSettings"
builder.AddConfiguredPathBase();

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
