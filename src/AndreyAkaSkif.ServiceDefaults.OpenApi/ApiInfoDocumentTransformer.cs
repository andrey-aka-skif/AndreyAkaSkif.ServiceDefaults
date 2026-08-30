using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
#if NET10_0_OR_GREATER
using Microsoft.OpenApi;
#else
using Microsoft.OpenApi.Models;
#endif

namespace AndreyAkaSkif.ServiceDefaults.OpenApi;

/// <summary>
/// Заполняет атрибуцию документа значениями секции <c>OpenApi</c>
/// </summary>
/// <remarks>
/// Сам генератор этого не делает: заголовок он выводит из имени сборки, версию — из имени
/// документа, а серверы не проставляет вовсе
/// </remarks>
internal sealed class ApiInfoDocumentTransformer(IOptions<OpenApiAppSettings> options)
    : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        var settings = options.Value;

        document.Info = new OpenApiInfo
        {
            Title = settings.Title,
            Version = settings.Version,
            Description = settings.Description
        };

        document.Servers = [.. settings.Servers.Select(server => new OpenApiServer { Url = server })];

        return Task.CompletedTask;
    }
}
