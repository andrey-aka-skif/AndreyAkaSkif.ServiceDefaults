using AndreyAkaSkif.ServiceDefaults.Settings;

namespace AndreyAkaSkif.ServiceDefaults.Routing;

internal sealed record RouteAppSettings : IValidatableSettingsObject
{
    public string PathBase { get; init; } = string.Empty;

    public void Validate()
    {
        // пустая строка - допустима
        if (PathBase == string.Empty)
            return;

        // строка не должна быть null
        if (PathBase is null)
            ThrowValidateException();

        // строка должна начинаться со слеша
        if (!PathBase!.StartsWith('/'))
            ThrowValidateException();

        // строка не должна содержать двойные слеши
        if (PathBase.Contains("//"))
            ThrowValidateException();

        char[] invalidChars = [
            '?',                // query-параметры
            '#',                // фрагменты
            '<',
            '>',
            '[',
            ']',
            '(',
            ')',
            '^',
            '`',
            '|',
            '\\',
            ':',
            '*',
            '\"',
            '\'',
            '%',
            '!',
            '@',
            ' ',
        ];

        if (PathBase.IndexOfAny(invalidChars) != -1)
            ThrowValidateException();
    }

    private void ThrowValidateException() =>
        throw new ArgumentException(
                $"Недопустимые символы в {nameof(RouteAppSettings)}:{nameof(PathBase)}: '{PathBase}'",
                nameof(PathBase));

}
