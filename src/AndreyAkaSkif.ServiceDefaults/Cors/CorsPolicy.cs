namespace AndreyAkaSkif.ServiceDefaults.Cors;

internal sealed record CorsPolicy
{
    public string Name { get; init; } = string.Empty;
    public string[] Origins { get; init; } = [];
}
