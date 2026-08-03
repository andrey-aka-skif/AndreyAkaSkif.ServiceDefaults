namespace AndreyAkaSkif.ServiceDefaults.Samples.Api.Endpoints;

/// <summary>
/// Демонстрационный тип канала: используется как сегмент пути
/// </summary>
internal enum DemoChannel
{
    /// <summary>
    /// Ток по фазе A
    /// </summary>
    CurrentA,

    /// <summary>
    /// Напряжение по фазе A
    /// </summary>
    VoltageA,

    /// <summary>
    /// Крутящий момент
    /// </summary>
    Torque,
}
