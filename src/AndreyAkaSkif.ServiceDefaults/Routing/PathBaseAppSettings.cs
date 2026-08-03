using System.Text.RegularExpressions;
using AndreyAkaSkif.ServiceDefaults.Settings;

namespace AndreyAkaSkif.ServiceDefaults.Routing;

internal sealed partial record PathBaseAppSettings : IValidatableSettingsObject
{
    /// <summary>
    /// Описание допустимого формата для сообщения об ошибке
    /// </summary>
    private const string EXPECTED_FORMAT =
        "ожидается пустая строка или путь вида /segment[/segment...] " +
        "из символов A-Za-z0-9 - . _ ~";

    public string Path { get; init; } = string.Empty;

    // исключение бросается здесь, а не во вспомогательном методе: CA2208 требует,
    // чтобы paramName совпадал с именем параметра метода, а у Validate их нет
    public void Validate()
    {
        if (Path is null || !PathBasePattern().IsMatch(Path))
            throw new ArgumentException(
                $"Недопустимое значение {nameof(PathBaseAppSettings)}:{nameof(Path)} " +
                $"'{Path}': {EXPECTED_FORMAT}",
                nameof(Path));
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
