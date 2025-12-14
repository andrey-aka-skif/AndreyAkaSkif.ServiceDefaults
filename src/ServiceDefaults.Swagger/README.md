# AndreyAkaSkif.ServiceDefaults.Swagger

Расширение для `AndeyAkaSkif.ServiceDefaults`, добавляющее единообразную конфигурацию OpenAPI спецификации с использованием Swagger UI.

## Установка
```sh
dotnet add package AndeyAkaSkif.ServiceDefaults.Swagger
```

## Возможности
- конфигурация OpenAPI спецификации и Swagger UI с параметрами по умолчанию (`AddDefaultOpenApiViaSwagger()`, `UseDefaultOpenApiViaSwagger()`);
- конфигурация OpenAPI спецификации и Swagger UI на основе конфигурации (`AddConfiguredOpenApiViaSwagger()`, `UseConfiguredOpenApiViaSwagger()`)
- отображение конечной точки HealthCheck в Swagger UI (`AddHealthChecksSwagger()`).

## Пример использования
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultOpenApiViaSwagger();

// раздельная регистрация Health Checks сервисов и отображение конечной точки в Swagger UI
builder.AddHealthChecks();
builder.AddHealthChecksSwagger();

// или единая регистрация Health Checks сервисов и отображение конечной точки в Swagger UI
builder.AddHealthChecksWithSwagger();

var app = builder.Build();

app.UseDefaultOpenApiViaSwagger();

// добавление конечной точки HealthCheck в конвейер обработки запросов
app.MapHealthChecksEndpoint();

app.Run();
```

## Особенности
### Конфигурация
Требует наличия в конфигурации секции "SwaggerAppSettings" со следующей структурой:
```json
"SwaggerAppSettings": {
    "Title": "string",            // Название API
    "Description": "string",      // Описание API
    "ApiVersion": "string",       // Версия API
    "Servers": [                  // Массив серверов (URL)
        "string"
    ]
}
```

В случае отсутствия обязательных параметров или невалидных данных в секции конфигурации будет выброшено исключение `ArgumentException`.
**Следует обратить внимание,** что при использовании файлов конфигурации секция должна быть указана в основном файле `appsettings.json`.
**Не следует** выносить секцию в файлы, содержащие среду. Например, в `appsettings.Development.json`.

### Транзитивные зависимости
При использовании методов из библиотеки `ServiceDefaults.Swagger` всегда устанавливается пакет `Swashbuckle.AspNetCore.Annotations`.
Даже, если в проекте не используются MVC контроллеры, а только Minimal Api.

### Отображение конечной точки HealthCheck в Swagger UI
Метод `AddHealthChecksSwagger()` только добавляет описание конечной точки в документацию Swagger.
Для функционирования конечной точки необходимо включить HealthCheck сервисы и добавить HealthCheck middleware в конвейер обработки запросов
В ином случае конечная точка будет неактивна. Соответствующий пункт Swagger UI будет возвращать ошибку `404 Not Found`.
Включение HealthCheck сервисов осуществляется с помощью метода `AddHealthChecks()` из пакета `AndreyAkaSkif.ServiceDefaults`.
Добавление HealthCheck middleware осуществляется с помощью метода `MapHealthChecksEndpoint()` из пакета `AndreyAkaSkif.ServiceDefaults`:
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddHealthChecks();
builder.AddHealthChecksSwagger();

var app = builder.Build();

app.MapHealthChecksEndpoint();

app.Run();
```

Альтернативно, можно использовать единый метод `AddHealthChecksWithSwagger()`, который включает регистрацию HealthCheck сервисов.

## Документация пакета
Полное описание пакета и другие примеры:
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults
