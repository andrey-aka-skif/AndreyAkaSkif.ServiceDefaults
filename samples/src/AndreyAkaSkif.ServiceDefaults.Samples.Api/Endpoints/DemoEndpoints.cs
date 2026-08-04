using AndreyAkaSkif.ServiceDefaults.Samples.Api.AppSettings;
using AndreyAkaSkif.ServiceDefaults.Samples.Api.Infrastructure.GitHub;
using AndreyAkaSkif.ServiceDefaults.Samples.Api.Services;

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

        // Настройки резолвятся из DI голым типом, без IOptions: их туда положил
        // AddAppSettings<DemoAppSettings, DemoAppSettingsValidator>()
        demo.MapGet("/greeting", (DemoAppSettings settings) => Results.Ok(new { settings.Greeting }))
            .WithName("GetGreeting")
            .WithSummary("Приветствие из секции конфигурации DemoAppSettings");

        // Соседняя точка показывает вторую роль: сервис получает не DemoAppSettings,
        // а собственный GreetingServiceArgs, который лежит рядом с ним и про
        // конфигурацию ничего не знает. Связывает их AddAppServices()
        demo.MapGet("/greeting/{name}", (string name, IGreetingService greetings) =>
                Results.Ok(new { greeting = greetings.Greet(name) }))
            .WithName("GetPersonalGreeting")
            .WithSummary("Приветствие по имени: сервис с собственным объектом-параметром");

        demo.MapGet("/echo", (string message) => Results.Ok(new { message }))
            .WithName("GetEcho")
            .WithSummary("Эхо query-параметра");

        // Типизированный API-клиент из Infrastructure/. Его зарегистрировал
        // AddApiClient<GitHubApiClient, GitHubApiClientOptions>(), адрес пришёл
        // из секции "GitHubApiClientOptions". Точка требует доступа в интернет
        demo.MapGet("/repository", async (GitHubApiClient gitHub, CancellationToken cancellationToken) =>
            {
                var repository = await gitHub.GetRepositoryAsync(
                    "andrey-aka-skif",
                    "AndreyAkaSkif.ServiceDefaults",
                    cancellationToken);

                // GitHubRepository наружу не отдаётся: это тип внешнего API со своими
                // именами полей. Ответ сервиса собирается здесь, поэтому snake_case
                // GitHub не протекает в контракт этой конечной точки
                return repository is null
                    ? Results.NotFound()
                    : Results.Ok(new
                    {
                        repository.FullName,
                        repository.Description,
                        repository.StargazersCount,
                    });
            })
            .WithName("GetRepository")
            .WithSummary("Вызов внешнего REST API через типизированный клиент");

        // Ограничение "demoChannel" зарегистрировал AddEnumRouteConstraint<DemoChannel>().
        // Значение вне перечисления сюда не доходит: маршрут не выбирается, ответ 404
        demo.MapGet("/channel/{channel:demoChannel}", (DemoChannel channel) => Results.Ok(new { channel }))
            .WithName("GetChannel")
            .WithSummary("Перечисление как сегмент пути: /demo/channel/CurrentA");

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
