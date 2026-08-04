using AndreyAkaSkif.ServiceDefaults.Settings;

// переезд на конвейер параметров выполняется отдельным шагом
#pragma warning disable CS0618

namespace AndreyAkaSkif.ServiceDefaults.Cors;

internal sealed record CorsPolicy : IValidatableSettingsObject
{
    public string Name { get; init; } = string.Empty;
    public string[] Origins { get; init; } = [];

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException($"Требуется {nameof(CorsPolicy)}:{nameof(Name)}");

        if (Origins == null || Origins.Length == 0)
            throw new ArgumentException($"Требуется хотя бы один источник в {nameof(CorsPolicy)}:{nameof(Origins)}");
    }
}
