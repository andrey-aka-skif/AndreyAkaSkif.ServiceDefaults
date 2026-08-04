# AndreyAkaSkif.ServiceDefaults.Swagger

Расширение для `AndreyAkaSkif.ServiceDefaults`, добавляющее единообразную конфигурацию OpenAPI спецификации с использованием Swagger UI.

## Установка
```sh
dotnet add package AndreyAkaSkif.ServiceDefaults.Swagger
```

## Возможности
- конфигурация OpenAPI спецификации и Swagger UI с параметрами по умолчанию (`AddDefaultOpenApiViaSwagger()`, `UseDefaultOpenApiViaSwagger()`);
- конфигурация OpenAPI спецификации и Swagger UI на основе конфигурации (`AddConfiguredOpenApiViaSwagger()`, `UseConfiguredOpenApiViaSwagger()`)
- отображение конечной точки проверки жизнеспособности в Swagger UI (`AddHealthCheckEndpointSwagger()`);
- совместная регистрация сервисов конечной точки проверки жизнеспособности и её отображения
  в Swagger UI одним вызовом (`AddHealthCheckEndpointWithSwagger()`).

## Пример использования
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultOpenApiViaSwagger();

// раздельная регистрация сервисов конечной точки и её отображения в Swagger UI
builder.AddHealthCheckEndpoint();
builder.AddHealthCheckEndpointSwagger();

// или единая регистрация сервисов конечной точки и её отображения в Swagger UI
builder.AddHealthCheckEndpointWithSwagger();

var app = builder.Build();

app.UseDefaultOpenApiViaSwagger();

// добавление конечной точки проверки жизнеспособности в конвейер обработки запросов
app.MapHealthCheckEndpoint();

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
    "Servers": [                  // Массив адресов серверов, необязательный
        "string"
    ]
}
```

В случае отсутствия обязательных параметров или невалидных данных в секции конфигурации приложение не поднимется: на старте хоста будет выброшено исключение `OptionsValidationException` со списком всех нарушенных правил.
**Следует обратить внимание,** что при использовании файлов конфигурации секция должна быть указана в основном файле `appsettings.json`.
**Не следует** выносить секцию в файлы, содержащие среду. Например, в `appsettings.Development.json`.

### Адреса серверов
Адреса в `Servers` следует задавать относительными: `"/"`, `"/api"`.

Swagger UI берёт базовый адрес запросов из первого элемента списка, поэтому абсолютный адрес ломает работу UI везде, кроме прописанного адреса: за reverse proxy, на другом хосте, а также при открытии страницы по https рядом с http-адресом в конфигурации — браузер режет такой запрос как mixed content и отвечает `Failed to fetch` на каждой конечной точке.

Относительный адрес допускается OpenAPI 3, и UI резолвит его от адреса страницы: запрос остаётся same-origin, то есть CORS не участвует.

Список необязателен. Если он пуст или отсутствует, подставляется адрес `"/"`.

### ApiExplorer
Генерация спецификации строится поверх ApiExplorer, а для Minimal Api он не регистрируется сам.
`Add*`-методы пакета регистрируют его самостоятельно, поэтому отдельный вызов `AddEndpointsApiExplorer()` не требуется.
Если приложение вызывает этот метод для собственных задач, повторная регистрация безопасна: метод идемпотентен.

### Транзитивные зависимости
При использовании методов из библиотеки `ServiceDefaults.Swagger` всегда устанавливается пакет `Swashbuckle.AspNetCore.Annotations`.
Даже, если в проекте не используются MVC контроллеры, а только Minimal Api.

### Отображение конечной точки проверки жизнеспособности в Swagger UI
Метод `AddHealthCheckEndpointSwagger()` только добавляет описание конечной точки в документацию Swagger.
Для функционирования конечной точки необходимо включить HealthCheck сервисы и добавить HealthCheck middleware в конвейер обработки запросов
В ином случае конечная точка будет неактивна. Соответствующий пункт Swagger UI будет возвращать ошибку `404 Not Found`.
Включение HealthCheck сервисов осуществляется с помощью метода `AddHealthCheckEndpoint()` из пакета `AndreyAkaSkif.ServiceDefaults`.
Добавление HealthCheck middleware осуществляется с помощью метода `MapHealthCheckEndpoint()` из пакета `AndreyAkaSkif.ServiceDefaults`:
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddHealthCheckEndpoint();
builder.AddHealthCheckEndpointSwagger();

var app = builder.Build();

app.MapHealthCheckEndpoint();

app.Run();
```

Альтернативно, можно использовать единый метод `AddHealthCheckEndpointWithSwagger()`, который включает регистрацию HealthCheck сервисов.

В Swagger UI описывается конечная точка `/health` (константа `HealthCheckDefaults.Endpoint`
из пакета `AndreyAkaSkif.ServiceDefaults`) с единственным ответом `200 Healthy`: конечная точка
не выполняет зарегистрированные проверки и отвечает успехом самим фактом ответа приложения.
Адрес не конфигурируется — он должен совпадать с адресом, который регистрирует
`MapHealthCheckEndpoint()`.
Подробнее о контракте — в README пакета `AndreyAkaSkif.ServiceDefaults`.

## Документация пакета
Полное описание пакета и другие примеры:
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults

## Сообщить о проблеме
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/issues
