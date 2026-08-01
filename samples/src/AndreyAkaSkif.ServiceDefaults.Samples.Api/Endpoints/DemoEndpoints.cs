using AndreyAkaSkif.ServiceDefaults.Samples.Api.Settings;

namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.Endpoints;

/// <summary>
/// Демонстрационные конечные точки приложения
/// </summary>
internal static class DemoEndpoints
{
    /// <summary>
    /// Добавить демонстрационные конечные точки
    /// </summary>
    public static IEndpointRouteBuilder MapDemoEndpoints(this IEndpointRouteBuilder app)
    {
        var demo = app.MapGroup("/demo").WithTags("Demo");

        // Настройки резолвятся из DI: их туда положил
        // AddServiceArgFromValidatedSettingsObject<DemoAppSettings>()
        demo.MapGet("/greeting", (DemoAppSettings settings) => Results.Ok(new { settings.Greeting }))
            .WithName("GetGreeting")
            .WithSummary("Приветствие из секции конфигурации DemoAppSettings");

        demo.MapGet("/echo", (string message) => Results.Ok(new { message }))
            .WithName("GetEcho")
            .WithSummary("Эхо query-параметра");

        // Исключение перехватывает middleware из UseErrorHandling() и возвращает
        // ProblemDetails. В Development к нему добавляется поле "exception" —
        // это делает AddExtendedErrorHandling()
        demo.MapGet("/boom", void () => throw new InvalidOperationException("Демонстрация обработки ошибок"))
            .WithName("GetBoom")
            .WithSummary("Необработанное исключение: демонстрация ProblemDetails")
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}
