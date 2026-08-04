namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.Services;

/// <summary>
/// Аргументы <see cref="GreetingService"/>
/// </summary>
/// <remarks>
/// Лежит рядом с сервисом, а не среди объектов настроек: это часть домена. Тип не знает
/// ни про <c>IOptions</c>, ни про имена секций конфигурации, ни про
/// <c>IValidatableSettingsObject</c> — он лишь объявляет, что сервису нужно для работы.
/// Откуда возьмётся значение, решает composition root, см.
/// <c>AppConfiguration/AppServicesConfigureExtensions.cs</c>
/// </remarks>
/// <param name="Greeting">Приветствие, которым сервис открывает обращение</param>
internal sealed record GreetingServiceArgs(string Greeting);
