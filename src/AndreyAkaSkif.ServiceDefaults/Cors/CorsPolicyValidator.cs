using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Cors;

internal sealed class CorsPolicyValidator : IValidateOptions<CorsPolicy>
{
    public ValidateOptionsResult Validate(string? name, CorsPolicy options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Name))
            failures.Add($"Требуется {nameof(CorsPolicy)}:{nameof(CorsPolicy.Name)}");

        if (options.Origins is null || options.Origins.Length == 0)
            failures.Add(
                $"Требуется хотя бы один источник в {nameof(CorsPolicy)}:{nameof(CorsPolicy.Origins)}");

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
