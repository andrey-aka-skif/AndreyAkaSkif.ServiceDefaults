using Microsoft.AspNetCore.OpenApi;
#if NET10_0_OR_GREATER
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
#else
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
#endif

namespace AndreyAkaSkif.ServiceDefaults.OpenApi;

/// <summary>
/// Добавляет в документ описание конечной точки проверки жизнеспособности
/// </summary>
/// <remarks>
/// <para>
/// Конечная точка регистрируется промежуточным ПО HealthCheck, а не обработчиком minimal API,
/// поэтому ApiExplorer её не видит и генератор о ней не знает. Описание добавляется вручную
/// </para>
/// <para>
/// <strong>
/// Контракт конечной точки требует актуализации в случае изменения реализации
/// HealthCheck middleware
/// </strong>
/// </para>
/// <para>
/// Сам контракт собирается один раз, а различия между Microsoft.OpenApi 1.x и 2.x вынесены
/// в приватные методы внизу файла: расходятся только способы задать метод, теги и тип схемы
/// </para>
/// </remarks>
internal sealed class HealthCheckDocumentTransformer(string endpoint) : IOpenApiDocumentTransformer
{
    private const string Description =
        "Проверка жизнеспособности приложения для оркестратора. "
        + "Отображение конечной точки, добавляемой через HealthCheck middleware";

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        var operation = new OpenApiOperation
        {
            Summary = "Health Check",
            Description = Description,
            Tags = CreateTags(),
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "Healthy",
                    Content = new Dictionary<string, OpenApiMediaType>(StringComparer.Ordinal)
                    {
                        ["text/plain"] = new OpenApiMediaType { Schema = CreateHealthySchema() }
                    }
                }
            }
        };

        var path = new OpenApiPathItem();
        AddGetOperation(path, operation);

        // документ приходит трансформеру в том виде, в каком его собрал генератор,
        // а у пустого документа коллекция путей не создана
        document.Paths ??= [];
        document.Paths.Add(endpoint, path);

        return Task.CompletedTask;
    }

#if NET10_0_OR_GREATER

    // Microsoft.OpenApi 2.x: теги операции — ссылки, тип схемы — флаговое перечисление,
    // значения enum — узлы JSON, а метод описывается объектом HttpMethod

    private static HashSet<OpenApiTagReference> CreateTags()
        => [new OpenApiTagReference("Health")];

    private static OpenApiSchema CreateHealthySchema()
        => new()
        {
            Type = JsonSchemaType.String,
            Enum = [JsonValue.Create("Healthy")]
        };

    private static void AddGetOperation(OpenApiPathItem path, OpenApiOperation operation)
        => path.Operations = new Dictionary<HttpMethod, OpenApiOperation> { [HttpMethod.Get] = operation };

#else

    // Microsoft.OpenApi 1.x: теги операции — объекты, тип схемы — строка,
    // значения enum — обёртки OpenApiAny, а метод описывается перечислением

    private static List<OpenApiTag> CreateTags()
        => [new OpenApiTag { Name = "Health" }];

    private static OpenApiSchema CreateHealthySchema()
        => new()
        {
            Type = "string",
            Enum = [new OpenApiString("Healthy")]
        };

    private static void AddGetOperation(OpenApiPathItem path, OpenApiOperation operation)
        => path.Operations.Add(OperationType.Get, operation);

#endif
}
