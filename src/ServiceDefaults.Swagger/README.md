# Service Defaults OpenAPI via Swagger

Расширение для `AndeyAkaSkif.ServiceDefaults`, добавляющее единообразную конфигурацию OpenAPI спецификации с использованием Swagger UI.

## Установка
```sh
dotnet add package AndeyAkaSkif.ServiceDefaults.Swagger
```

## Возможности
- конфигурация OpenAPI спецификации и Swagger UI с параметрами по умолчанию (`AddDefaultOpenApiViaSwagger()`, `UseDefaultOpenApiViaSwagger()`);
- конфигурация OpenAPI спецификации и Swagger UI на основе конфигурации (`AddConfiguredOpenApiViaSwagger()`, `UseConfiguredOpenApiViaSwagger()`)

## Пример использования
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultOpenApiViaSwagger();

var app = builder.Build();
app.UseDefaultOpenApiViaSwagger();

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

Дополнительная информация - в основном репозитории проекта:
https://github.com/andrey-aka-skif/ServiceDefaults
