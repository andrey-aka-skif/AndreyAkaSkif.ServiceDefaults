namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.Services;

/// <summary>
/// Составляет персональное приветствие
/// </summary>
internal interface IGreetingService
{
    /// <summary>
    /// Приветствует по имени
    /// </summary>
    /// <param name="name">Имя адресата</param>
    string Greet(string name);
}
