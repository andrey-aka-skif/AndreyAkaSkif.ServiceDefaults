using AndreyAkaSkif.ServiceDefaults.HealthChecking;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AndreyAkaSkif.ServiceDefaults.Swagger;

/// <summary>
/// Фильтр документации Swagger для добавления HealthCheck конечной точки
/// </summary>
/// <remarks>
/// <strong>
/// Контракт конечной точки требует актуализации в случае изменения реализации HealthCheck middleware
/// </strong>
/// </remarks>
public class HealthChecksDocumentFilter : IDocumentFilter
{
    /// <summary>Применить фильтр к документации Swagger</summary>
    /// <param name="swaggerDoc">Документ Swagger</param>
    /// <param name="context">Контекст фильтра документа</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var path = new OpenApiPathItem();

        path.Operations.Add(OperationType.Get, new OpenApiOperation
        {
            Tags = [new() { Name = "Health" }],
            Summary = "Health Check",
            Description = $"Проверка жизнеспособности приложения для оркестратора. "
                + $"Отображение конечной точки {HealthCheckDefaults.Endpoint}, добавляемой через HealthCheck middleware",
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "Healthy",
                    Content =
                    {
                        ["text/plain"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "string",
                                Enum =
                                [
                                    new OpenApiString("Healthy")
                                ]
                            }
                        }
                    }
                }
            }
        });

        swaggerDoc.Paths.Add(HealthCheckDefaults.Endpoint, path);
    }
}
