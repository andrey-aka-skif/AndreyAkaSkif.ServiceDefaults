using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace AndreyAkaSkif.ServiceDefaults.Routing;

internal sealed partial class PathBaseAppSettingsValidator : IValidateOptions<PathBaseAppSettings>
{
    /// <summary>
    /// Описание допустимого формата для сообщения об ошибке
    /// </summary>
    private const string EXPECTED_FORMAT =
        "ожидается пустая строка или путь вида /segment[/segment...] " +
        "из символов A-Za-z0-9 - . _ ~";

    public ValidateOptionsResult Validate(string? name, PathBaseAppSettings options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Path is not null && PathBasePattern().IsMatch(options.Path))
            return ValidateOptionsResult.Success;

        return ValidateOptionsResult.Fail(
            $"Недопустимое значение {nameof(PathBaseAppSettings)}:{nameof(PathBaseAppSettings.Path)} " +
            $"'{options.Path}': {EXPECTED_FORMAT}");
    }

    /// <summary>
    /// Пустая строка, либо последовательность сегментов из unreserved-символов RFC 3986,
    /// каждый с ведущим слешем, с необязательным хвостовым слешем
    /// </summary>
    /// <remarks>
    /// <para>
    /// Класс символов записан явно: <c>\w</c> и <c>\d</c> в .NET учитывают Unicode
    /// и пропустили бы кириллицу.
    /// </para>
    /// <para>
    /// Конец строки — <c>\z</c>, а не <c>$</c>: <c>$</c> совпадает и перед завершающим
    /// переводом строки, из-за чего "/api\n" считался бы валидным.
    /// </para>
    /// <para>
    /// Хвостовой слеш допустим: <c>UsePathBase</c> срезает его сам.
    /// </para>
    /// </remarks>
    [GeneratedRegex(@"\A(?:/[A-Za-z0-9._~-]+)*/?\z")]
    private static partial Regex PathBasePattern();
}
