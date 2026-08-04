namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.Services;

/// <inheritdoc cref="IGreetingService"/>
/// <remarks>
/// Зависит от <see cref="GreetingServiceArgs"/> — собственного типа домена, а не от
/// объекта секции конфигурации. Благодаря этому сервис остаётся независимым от того,
/// как приложение читает настройки
/// </remarks>
internal sealed class GreetingService(GreetingServiceArgs args) : IGreetingService
{
    public string Greet(string name) => $"{args.Greeting}, {name}!";
}
