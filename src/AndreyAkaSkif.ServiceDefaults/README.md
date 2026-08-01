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
    - политика CORS, настроенная через конфигурацию (`AddConfiguredCorsPolicy()` / `UseConfiguredCorsPolicy()`)
    - разрешительная политика CORS «всё со всех источников» (`AddPermissiveCorsPolicy()` / `UsePermissiveCorsPolicy()`)
    - саброутинг — базовый путь из конфигурации (`UseConfiguredPathBase()`)
    - обработка ошибок с использованием ProblemDetails — стандартная (`AddDefaultErrorHandling()`)
      и расширенная (`AddExtendedErrorHandling()`), подключение в конвейер — `UseErrorHandling()`
    - конечная точка проверки жизнеспособности приложения для оркестратора
      (`AddHealthCheckEndpoint()` / `MapHealthCheckEndpoint()`, адрес `/health`)
- Регистрация конфигураций и аргументов
    - регистрация валидируемых настроек в DI-контейнере (`AddServiceArgFromValidatedSettingsObject<T>()`)
    - регистрация готового экземпляра класса в DI-контейнере (`AddServiceArg<T>(instance)`)

## Пример
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguredCorsPolicy();      // добавление политики CORS, настроенной через конфигурацию
builder.AddExtendedErrorHandling();     // регистрация стандартной обработки ошибок с использованием ProblemDetails
builder.AddHealthCheckEndpoint();       // регистрация сервисов конечной точки проверки жизнеспособности приложения

builder.AddServiceArgFromValidatedSettingsObject<ExampleSettingsArgs>();    // регистрация валидированного объекта настроек как singleton в DI-контейнере

var app = builder.Build();

app.UseConfiguredCorsPolicy();          // подключение политики CORS, настроенной через конфигурацию
app.UseErrorHandling();                 // подключение промежуточного ПО для обработки исключений в конвейере запросов
app.UseConfiguredPathBase();            // добавление базового пути на основе конфигурации
app.MapHealthCheckEndpoint();           // добавление конечной точки проверки жизнеспособности (/health)

app.Run();
```

## CORS
Пакет предлагает две политики, взаимозаменяемые по вызову:

- `AddConfiguredCorsPolicy()` / `UseConfiguredCorsPolicy()` — источники берутся из конфигурации,
  разрешены любые методы и заголовки. Настройки читаются один раз при регистрации и попадают
  в DI-контейнер, откуда их берёт `Use*`-метод;
- `AddPermissiveCorsPolicy()` / `UsePermissiveCorsPolicy()` — политика `AllowAll`: любой источник,
  любой метод, любой заголовок. Конфигурация не требуется.

> [!WARNING]
> Разрешительная политика предназначена для локальной разработки. В продуктовой среде
> список источников следует задавать явно через `AddConfiguredCorsPolicy()`.

## Обработка ошибок
`AddDefaultErrorHandling()` — обёртка над стандартным `AddProblemDetails()`: ответ соответствует
RFC 7807 в любой среде.

`AddExtendedErrorHandling()` дополнительно добавляет в ответ поле `exception` с типом, сообщением
и stack trace исключения — **только в Development**. В остальных средах ответ тот же, что
у стандартного варианта.

Оба метода требуют вызова `UseErrorHandling()` — см. раздел «⚠️ Важно».

## Секции конфигурации
Имя секции всегда совпадает с именем типа настроек: привязка выполняется через
`IConfiguration.CreateValidated<T>()`, который читает секцию `typeof(T).Name`.
Правило действует и для собственных настроек приложения.

Политика CORS — секция `CorsPolicy`, оба параметра обязательны:
```json
"CorsPolicy": {
    "Name": "string",             // Имя политики
    "Origins": [                  // Разрешённые источники, минимум один
        "http://localhost:5001"
    ]
}
```

Базовый путь — секция `RouteAppSettings`:
```json
"RouteAppSettings": {
    "PathBase": "/api"
}
```

Пустая строка в `PathBase` допустима — базовый путь не добавляется. Непустое значение
должно начинаться с `/`, не содержать `//` и не содержать запрещённых символов:

```
? # < > [ ] ( ) ^ ` | \ : * " ' % ! @ и пробел
```

Иначе выбрасывается `ArgumentException`.

`UseConfiguredPathBase()` читает конфигурацию сам, парного `Add*`-метода у него нет —
это единственный метод пакета, выпадающий из схемы `Add*` / `Use*`.

Базовый путь не заменяет основной: приложение остаётся доступным и по исходным адресам.
В спецификацию OpenAPI базовый путь автоматически не попадает — его нужно указать
в списке серверов (см. `Servers` в README пакета `AndreyAkaSkif.ServiceDefaults.Swagger`).

## Собственные настройки
Чтобы подключить свой класс настроек, достаточно реализовать `IValidatableSettingsObject`
и зарегистрировать его одним вызовом:

```csharp
public sealed record DemoAppSettings : IValidatableSettingsObject
{
    public string Greeting { get; init; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Greeting))
            throw new ArgumentException($"Требуется {nameof(DemoAppSettings)}:{nameof(Greeting)}");
    }
}

builder.AddServiceArgFromValidatedSettingsObject<DemoAppSettings>();
```

Секция конфигурации в этом случае — `DemoAppSettings`. Готовый экземпляр доступен
из DI-контейнера как singleton. Рабочий образец — `DemoAppSettings` в `samples/`.

Если объект уже создан и валидировать его не нужно, используйте `AddServiceArg(instance)`.

## Настройки и fail fast
Методы `Add*`, читающие конфигурацию, привязывают секцию и валидируют объект настроек
**в момент вызова**, а не при первом разрешении сервиса из DI. В контейнер попадает уже готовый
экземпляр.

Поэтому некорректная конфигурация роняет приложение на старте, до вызова `builder.Build()`,
а не отложенно — при первом запросе, которому эти настройки понадобились.

## Health Checks
Пара методов `AddHealthCheckEndpoint()` / `MapHealthCheckEndpoint()` регистрирует минимальную
инфраструктуру ASP.NET Core Health Checks и одну конечную точку `/health`, достаточную для
использования Docker Compose, Kubernetes или другого оркестратора.

Назначение конечной точки — сообщить оркестратору, что приложение запустилось и принимает
HTTP-запросы. Никаких проверок она не выполняет и отвечает `200 Healthy` самим фактом ответа
приложения.

Адрес конечной точки — `/health`. Он задан константой `HealthCheckDefaults.Endpoint`
и не конфигурируется: то же значение использует фильтр документации Swagger из пакета
`AndreyAkaSkif.ServiceDefaults.Swagger`, и параметризация пути развела бы их между собой.
Если нужен другой адрес, добавьте свою конечную точку вызовом `app.MapHealthChecks()`
вместо `MapHealthCheckEndpoint()`.

Методы **не предназначены** для регистрации пользовательских проверок.
Для добавления проверок БД, Redis и других зависимостей используйте стандартный API
`builder.Services.AddHealthChecks()`.
Такие проверки сознательно не попадают в `/health`: недоступность зависимости не является поводом
перезапускать работающее приложение. Если проверки зависимостей нужно опубликовать, добавьте
для них отдельную конечную точку вызовом `app.MapHealthChecks()`:

```csharp
builder.AddHealthCheckEndpoint();                       // /health — приложение живо
builder.Services.AddHealthChecks()                      // проверки зависимостей
       .AddNpgSql(connectionString);

var app = builder.Build();

app.MapHealthCheckEndpoint();                           // /health
app.MapHealthChecks("/ready");                          // /ready — зависимости доступны
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

## Сообщить о проблеме
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/issues
