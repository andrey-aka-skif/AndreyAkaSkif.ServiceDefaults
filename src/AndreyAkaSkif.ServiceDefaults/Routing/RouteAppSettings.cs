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

        // строка должна быть корректно сформированной и подходящей для построения относительного Uri
        if (!Uri.IsWellFormedUriString(PathBase, UriKind.Relative))
            ThrowValidateException();

        // строка не должна быть абсолютным путем, UNC, путем к файлу или localhost
        if (Uri.TryCreate(PathBase, UriKind.Absolute, out Uri? testUriResult))
        {
            if (testUriResult is not null)
            {
                if (testUriResult.IsAbsoluteUri
                    || testUriResult.IsUnc
                    || testUriResult.IsFile
                    || testUriResult.IsLoopback)
                {
                    ThrowValidateException();
                }
            }
        }

        string fullUri = $"http://localhost{PathBase}";

        // итоговый подмаршрут должен быть валидным
        if (!Uri.TryCreate(fullUri, UriKind.Absolute, out Uri? uriResult))
            ThrowValidateException();

        // итоговый подмаршрут не должен содержать query-параметров и фрагментов
        if (!string.IsNullOrEmpty(uriResult!.Query) || !string.IsNullOrEmpty(uriResult.Fragment))
            ThrowValidateException();
    }

    private void ThrowValidateException() =>
        throw new ArgumentException(
                $"Недопустимые символы в {nameof(RouteAppSettings)}:{nameof(PathBase)}: '{PathBase}'",
                nameof(PathBase));

}
