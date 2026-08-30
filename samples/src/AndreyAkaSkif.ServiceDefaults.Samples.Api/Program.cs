using AndreyAkaSkif.ServiceDefaults.Cors;
using AndreyAkaSkif.ServiceDefaults.ErrorHandling;
using AndreyAkaSkif.ServiceDefaults.HealthChecking;
using AndreyAkaSkif.ServiceDefaults.OpenApi;
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
builder.AddAllAppSettings();
builder.AddAppDbContexts();
builder.AddAppServices();
builder.AddAppHttpClients();
builder.AddAppRouteConstraints();

// ProblemDetails. В Development в ответ добавляется поле "exception"
builder.AddExtendedErrorHandling();

// Генерация спецификации OpenApi. Конфигурация — секция "OpenApi"
builder.AddConfiguredOpenApi();

// Показ спецификации в Swagger UI. Конфигурация — секция "Swagger".
// Пакет отвечает только за показ: спецификацию он берёт по адресу из конфигурации.
// Чтобы обойтись без UI, достаточно убрать эту пару вызовов и ссылку на пакет
builder.AddSwaggerUi();

// Политика CORS. Конфигурация — секция "CorsPolicy"
builder.AddConfiguredCorsPolicy();

// Базовый путь. Конфигурация — секция "PathBaseAppSettings"
builder.AddConfiguredPathBase();

// Конечная точка /health и её описание в спецификации. Саму точку добавляет
// MapHealthCheckEndpoint(), описание живёт в пакете спецификации
builder.AddHealthCheckEndpoint();
builder.AddHealthCheckEndpointDescription();

var app = builder.Build();

app.MapDemoEndpoints();

app.UseErrorHandling();
app.UseConfiguredOpenApi();
app.UseSwaggerUi();
app.UseConfiguredCorsPolicy();
app.UseConfiguredPathBase();
app.MapHealthCheckEndpoint();

app.Run();
