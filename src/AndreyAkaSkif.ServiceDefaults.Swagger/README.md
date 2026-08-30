# AndreyAkaSkif.ServiceDefaults.Swagger

Показ спецификации OpenAPI через Swagger UI.

Пакет только показывает спецификацию и не участвует в её создании: ему задают адрес,
по которому документ доступен, а чем и где документ сгенерирован — ему безразлично.
Зависимость у пакета одна — `Swashbuckle.AspNetCore.SwaggerUI`.

## Установка
```sh
dotnet add package AndreyAkaSkif.ServiceDefaults.Swagger
```

Поддерживаются `net9.0` и `net10.0`.

## Возможности
- Swagger UI для спецификации по указанному адресу (`AddSwaggerUi()`, `UseSwaggerUi()`);
- перенаправление с корня приложения на страницу UI;
- управление показом по средам.

## Пример использования
Обычный сценарий — рядом с пакетом `AndreyAkaSkif.ServiceDefaults.OpenApi`, который
спецификацию генерирует и раздаёт:

```csharp
var builder = WebApplication.CreateBuilder(args);

// генерация спецификации: пакет AndreyAkaSkif.ServiceDefaults.OpenApi
builder.AddConfiguredOpenApi();

// показ: этот пакет
builder.AddSwaggerUi();

var app = builder.Build();

app.UseConfiguredOpenApi();
app.UseSwaggerUi();

app.Run();
```

Спецификацию может отдавать и любой другой источник — собственный обработчик приложения,
статический файл, раздаваемый по HTTP, сторонний сервис. Пакету достаточно адреса, по
которому документ доступен браузеру:

```json
"Swagger": {
    "Url": "https://api.example.com/openapi/v1.json",
    "Name": "Example API"
}
```

## Особенности
### Конфигурация
Секция `Swagger` необязательна целиком: без неё UI показывает документ по адресу
`/openapi/v1.json` — тому, по которому его раздаёт `MapOpenApi()` при имени документа
по умолчанию.

```json
"Swagger": {
    "Url": "/openapi/v1.json",
    "Name": "Demo API",
    "Visibility": "ByEnvironment"
}
```

| Ключ | По умолчанию | Что это |
| --- | --- | --- |
| `Url` | `/openapi/v1.json` | Адрес спецификации |
| `Name` | значение `Url` | Имя документа в интерфейсе |
| `Visibility` | `ByEnvironment` | Показ UI |

Заданный пустым `Url` роняет приложение на старте хоста: UI поднялся бы, но спецификацию
не нашёл.

### Показ UI

| Значение `Visibility` | Поведение |
| --- | --- |
| `ByEnvironment` (умолчание) | Вне `Production` UI показывается, в `Production` — нет |
| `Always` | Показывается в любой среде |
| `Never` | Не показывается ни в какой |

В среде, где UI не показывается, не формируется и перенаправление с корня: `/` вернёт
`404 Not Found`, как и `/swagger`.

### Показ и раздача документа управляются раздельно
Этот пакет отвечает только за страницу UI. Доступность самого документа определяет тот,
кто его раздаёт: у пакета `AndreyAkaSkif.ServiceDefaults.OpenApi` это ключ
`OpenApi:Visibility` с такими же тремя состояниями.

Из этого следует практическое правило: **чтобы открыть UI в `Production`, включить нужно
оба ключа**. При включённом `Swagger:Visibility` и выключенном `OpenApi:Visibility`
страница откроется, но документ по своему адресу вернёт `404`, и UI останется пустым.

### Адрес спецификации
`Url` — это адрес, который запрашивает браузер, а не путь в файловой системе.

Относительный адрес (`/openapi/v1.json`) резолвится браузером от адреса страницы, поэтому
работает и за reverse proxy, и на любом хосте.

Абсолютный адрес допустим — так показывают чужую спецификацию, — но запрос к ней уходит
кросс-доменным, и отдающая сторона должна разрешать его через CORS.

Путь в файловой системе и схема `file://` не работают ни в какой ОС: Swagger UI считает
такое значение относительным адресом и достраивает его к адресу страницы — получается
запрос вида `/swagger/file:///D:/.../openapi.json` и ответ `404`. Чтобы показать файл,
его нужно раздать по HTTP — например `UseStaticFiles()` — и указать сетевой адрес.
Отсюда и имя ключа: `Url`, а не `Uri`. Поддерживается именно сетевой адрес, а не
произвольный идентификатор ресурса; так же называют этот параметр Swagger UI
(`urls[].url`) и Swashbuckle (`SwaggerEndpoint(url, name)`).

### Аннотации Swashbuckle не поставляются
Пакет не содержит генератор Swashbuckle, а значит и `Swashbuckle.AspNetCore.Annotations`:
атрибуты вроде `[SwaggerOperation]` в спецификацию не попадут. Описания операций задаются
средствами фреймворка — `[EndpointSummary]`, `[EndpointDescription]`, `.WithSummary()`,
`.WithDescription()`: они лежат в метаданных и не зависят от того, чем сгенерирована
спецификация.

### Конечная точка проверки жизнеспособности
Конечную точку `/health` описывает в документе тот, кто документ генерирует. Для пакета
`AndreyAkaSkif.ServiceDefaults.OpenApi` это `AddHealthCheckEndpointDescription()` — см.
его README.

## Документация пакета
Полное описание пакета и другие примеры:
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults

## Сообщить о проблеме
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/issues
