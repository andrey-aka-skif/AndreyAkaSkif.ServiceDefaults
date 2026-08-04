namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.Services;

/// <summary>
/// Составляет персональное приветствие
/// </summary>
internal interface IGreetingService
{
    /// <summary>
    /// Поприветствовать по имени
    /// </summary>
    /// <param name="name">Имя адресата</param>
    string Greet(string name);
}
