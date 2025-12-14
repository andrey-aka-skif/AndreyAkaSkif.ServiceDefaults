# AndreyAkaSkif.ServiceDefaults
Библиотека содержит методы расширения для стандартизированной конфигурации ASP.NET Web-API приложений.

Библиотека создана как внутренний стандарт инфраструктуры моих сервисов.
Она оформлена как NuGet-пакет, чтобы не копировать один и тот же код в каждый проект.

## Установка

```sh
dotnet add package AndreyAkaSkif.ServiceDefaults
```

## Возможности
- Инициализация API окружения
    - настройка политик CORS
    - саброутинг (PathBase)
    - обработка ошибок с использованием ProblemDetails
    - упрощенный Health Checks (конечная точка `/health`)
- Регистрация конфигураций и аргументов
    - упрощённая регистрация валидируемых настроек в DI контейнере через единый extension-метод
    - упрощенная регистрация произвольного экземпляра класса в DI контейнере через единый extension-метод

## Пример
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguredCorsPolicy();      // добавление политики CORS, настроенной через конфигурацию
builder.AddExtendedErrorHandling();     // регистрация стандартной обработки ошибок с использованием ProblemDetails

builder.AddServiceArgFromValidatedSettingsObject<ExampleSettingsArgs>();    // регистрация валидированного объекта настроек как singleton в DI-контейнере

var app = builder.Build();

app.UseConfiguredCorsPolicy();          // подключение политики CORS, настроенной через конфигурацию
app.UseErrorHandling();                 // подключение промежуточного ПО для обработки исключений в конвейере запросов
app.UseConfiguredPathBase();            // добавление базового пути на основе конфигурации
app.UseSimpleHealthCheck();             // добавление упрощенной проверки состояния

app.Run();
```

## ⚠️ Важно
Метод `UseErrorHandling()` обязательно должен быть вызван при использовании методов
`AddDefaultErrorHandling()` или `AddExtendedErrorHandling()`.
Если этого не сделать, поведение может оказаться неожиданным.

При использовании Swagger в ответе вернется полный stack trace ошибки, который может содержать конфиденциальную информацию.

При вызове из стороннего сервиса `ProblemDetails` не будет возвращен.

Причина в том, что метод `AddProblemDetails()` только регистрирует сервис обработки ошибок.
Непосредственно включает перехват исключений метод `UseExceptionHandler()`.

## Документация пакета
Полное описание пакета и другие примеры:
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults
