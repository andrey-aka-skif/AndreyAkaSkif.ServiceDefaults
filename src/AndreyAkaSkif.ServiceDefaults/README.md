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
    - саброутинг — базовый путь из конфигурации (`AddConfiguredPathBase()` / `UseConfiguredPathBase()`)
    - перечисление как сегмент пути со строковой сериализацией (`AddEnumRouteConstraint<T>()`);
      отдельно доступны сериализация перечислений именами (`AddStringEnumJsonSerialization()`)
      и регистрация собственного ограничения маршрута (`AddRouteConstraint<T>(name)`)
    - обработка ошибок с использованием ProblemDetails — стандартная (`AddDefaultErrorHandling()`)
      и расширенная (`AddExtendedErrorHandling()`), подключение в конвейер — `UseErrorHandling()`
    - конечная точка проверки жизнеспособности приложения для оркестратора
      (`AddHealthCheckEndpoint()` / `MapHealthCheckEndpoint()`, адрес `/health`)
- Регистрация настроек
    - настройки инфраструктуры, доступные как `IOptions<T>` (`AddValidatedOptions<T, TValidator>()`)
    - настройки приложения, доступные значением, без `IOptions<T>` (`AddAppSettings<T, TValidator>()`)
- Взаимодействие с внешними сервисами
    - типизированный API-клиент с адресом из конфигурации (`AddApiClient<TClient, TOptions>()`)

## Пример
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguredCorsPolicy();      // добавление политики CORS, настроенной через конфигурацию
builder.AddConfiguredPathBase();        // регистрация настроек базового пути
builder.AddExtendedErrorHandling();     // регистрация стандартной обработки ошибок с использованием ProblemDetails
builder.AddHealthCheckEndpoint();       // регистрация сервисов конечной точки проверки жизнеспособности приложения

builder.AddAppSettings<DemoAppSettings, DemoAppSettingsValidator>();       // регистрация настроек приложения: в DI попадает значение, а не IOptions<T>

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
  разрешены любые методы и заголовки. Настройки попадают в конвейер параметров, откуда
  их берут и сама политика, и `Use*`-метод;
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

## Типизированные API-клиенты
API-клиент — инфраструктурный адаптер. Он инкапсулирует знание о внешнем сервисе: адреса
конечных точек, структуру JSON, типы ответов и требования к запросу вроде обязательных
заголовков. Прикладной код вызывает метод и получает готовый объект, про HTTP не зная
ничего. Ближайшая аналогия — клиенты, которые генерируют `openapi-generator`
или `@hey-api/openapi-ts`, только написанные руками.

Библиотека берёт на себя одно: адрес внешнего сервиса. Настройки клиента реализуют
`IHttpApiClientOptions`, а сам клиент принимает `HttpClient` через конструктор:

```csharp
public sealed class GitHubApiClientOptions : IHttpApiClientOptions
{
    public string BaseAddress { get; set; } = string.Empty;
}

internal sealed class GitHubApiClient(HttpClient httpClient)
{
    public async Task<GitHubRepository?> GetRepositoryAsync(string owner, string name)
        => await httpClient.GetFromJsonAsync<GitHubRepository>($"repos/{owner}/{name}");
}

builder.AddApiClient<GitHubApiClient, GitHubApiClientOptions>();
```

Секция конфигурации — `GitHubApiClientOptions`:

```json
"GitHubApiClientOptions": {
    "BaseAddress": "https://api.github.com/"
}
```

Адрес должен быть абсолютным, со схемой `http` или `https`. Схема проверяется явно:
одной проверки на абсолютность мало — в Unix путь вида `/api` разбирается как путь
файловой системы и даёт валидный `file:///api`, а в Windows не разбирается вовсе, так что
без явной проверки поведение зависело бы от платформы.

Хвостовой слеш в адресе значим: без него последний сегмент будет отброшен при разрешении
относительного пути запроса. Так устроен `Uri`, а не библиотека.

Наследоваться от базового класса не нужно, интерфейс требует ровно одно свойство. Всё
остальное — ключи, версии API, идентификаторы — объявляется в том же классе настроек
и библиотеки не касается: клиент получает свои настройки как `IOptions<T>` обычным
способом.

### Что настраивается в самом клиенте
Заголовки, обработчики и прочая настройка `HttpClient` в сигнатуру метода не вынесены.
Знание о внешнем API принадлежит клиенту, поэтому там ему и место — GitHub, например,
отвечает `403` на запрос без `User-Agent`:

```csharp
public GitHubApiClient(HttpClient httpClient)
{
    httpClient.DefaultRequestHeaders.UserAgent.Add(
        new ProductInfoHeaderValue("MyService", "1.0"));

    _httpClient = httpClient;
}
```

Если нужен `DelegatingHandler` или политика устойчивости, регистрация дополняется штатным
API — `AddHttpClient` аддитивен, повторный вызов настраивает того же клиента:

```csharp
builder.AddApiClient<GitHubApiClient, GitHubApiClientOptions>();

builder.Services
       .AddHttpClient<GitHubApiClient>()
       .AddHttpMessageHandler<AuthorizationHandler>();
```

### Почему здесь `IOptions<T>`, а не развёрнутое значение
Настройки приложения пакет отдаёт значением, чтобы не тащить `IOptions<T>` в домен —
см. «Собственные настройки». Конфигурация HTTP-клиента доменным знанием не является:
это параметр инфраструктурного адаптера, который живёт по ту же сторону границы,
что и сам `HttpClient`. Разворачивать значение здесь незачем.

Граница проходит именно здесь: `IOptions<T>` напрямую — в инфраструктуре (политика CORS,
Swagger, базовый путь, настройки API-клиентов), развёрнутое значение — в домене.

## Перечисление как сегмент пути
Перечисление в маршруте по умолчанию неинформативно: спецификация OpenAPI объявляет его
целочисленным, Swagger UI предлагает ввести `0`, `1`, `2`, и в адресе оказывается
`/channel/2` вместо `/channel/Torque`. Решается одним вызовом:

```csharp
builder.AddEnumRouteConstraint<ChannelType>();

app.MapGet("channel/{channel:channelType}", (ChannelType channel) => channel);
```

Имя ограничения по умолчанию — имя типа со строчной первой буквы, для `ChannelType` это
`channelType`. Другое имя задаётся параметром: `AddEnumRouteConstraint<ChannelType>("channel")`.

Поведение:

| Запрос | Результат |
| --- | --- |
| `/channel/Torque` | 200 |
| `/channel/torque` | 200 — сопоставление регистронезависимо |
| `/channel/2` | 200 — числовые значения допустимы |
| `/channel/999` | 404 — значения нет в перечислении |
| `/channel/unknown` | 404 |

Значение приводится к каноническому имени элемента до привязки аргумента. Это важно:
привязка в минимальных API регистрозависима, поэтому без приведения `/channel/torque`
совпал бы с маршрутом и упал с 400 при разборе аргумента. Некорректное значение даёт 404
от маршрутизации, а не 400 от привязки.

Работает одинаково и для минимальных API, и для контроллеров: `ConstraintMap` живёт
в `RouteOptions`, который читает общий для обоих стилей резолвер ограничений.

```csharp
[HttpGet("channel/{channel:channelType}")]
public IActionResult Get(ChannelType channel) => Ok(channel);
```

Приведение к каноническому имени для контроллеров избыточно — привязка модели там идёт
через `EnumConverter` и уже регистронезависима, — но поведение от этого не меняется.

Заодно метод включает сериализацию перечислений именами — иначе тела ответов и
спецификация остались бы с числами и задача решилась бы наполовину. Настраиваются оба
набора параметров: `Microsoft.AspNetCore.Http.Json` для минимальных API и
`Microsoft.AspNetCore.Mvc.Json` для контроллеров. Второй нужен ещё и потому, что генератор
спецификации Swashbuckle читает именно его. Приложению без контроллеров это не вредит.

Если перечисления встречаются только в телах ответов и ограничение маршрута не нужно,
та же настройка доступна отдельно:

```csharp
builder.AddStringEnumJsonSerialization();
```

Вызов идемпотентен, поэтому регистрация нескольких перечислений его не дублирует.

Само ограничение — публичный тип `EnumRouteConstraint<TEnum>`; `AddEnumRouteConstraint<T>()`
всего лишь регистрирует его под именем по умолчанию и включает сериализацию именами.
Если сериализация именами не нужна, тип регистрируется напрямую:

```csharp
builder.AddRouteConstraint<EnumRouteConstraint<ChannelType>>("channelType");
```

### Собственные ограничения
`AddEnumRouteConstraint<T>()` построен поверх метода общего назначения, которым
регистрируется любая реализация `IRouteConstraint`. Сам по себе он задачу не решает —
он нужен, когда ограничение написано своё:

```csharp
public sealed class EvenNumberRouteConstraint : IRouteConstraint
{
    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection)
        => values.TryGetValue(routeKey, out var value)
           && int.TryParse(value?.ToString(), out var number)
           && number % 2 == 0;
}

builder.AddRouteConstraint<EvenNumberRouteConstraint>("even");

app.MapGet("items/{id:even}", (int id) => id);
```

Повторная регистрация того же типа под тем же именем допустима и ничего не меняет.
Регистрация другого типа под занятым именем — `InvalidOperationException`. Встроенные
ограничения (`int`, `guid`, `alpha` и прочие) остаются доступны.

## Секции конфигурации
Имя секции всегда совпадает с именем типа настроек: привязка выполняется через
`BindConfiguration(typeof(T).Name)`. Правило действует и для собственных настроек
приложения. Если конфигурация уже сложилась иначе, имя задаётся параметром
`sectionName`.

Политика CORS — секция `CorsPolicy`, оба параметра обязательны:
```json
"CorsPolicy": {
    "Name": "string",             // Имя политики
    "Origins": [                  // Разрешённые источники, минимум один
        "http://localhost:5001"
    ]
}
```

Правило действует и для настроек API-клиента: секция называется по имени класса настроек,
для `GitHubApiClientOptions` это `GitHubApiClientOptions`. Если конфигурация уже сложилась
иначе, имя задаётся параметром: `AddApiClient<GitHubApiClient, GitHubApiClientOptions>("GitHub")`.

Секция базового пути — `PathBaseAppSettings`, она описана в разделе «Базовый путь».

## Базовый путь
Базовый путь — секция `PathBaseAppSettings`:
```json
"PathBaseAppSettings": {
    "Path": "/api"
}
```

Допустимый формат — пустая строка либо путь вида `/segment[/segment...]`, где сегмент
состоит из unreserved-символов RFC 3986:

```
A-Za-z0-9 - . _ ~
```

Пустая строка означает, что базовый путь не добавляется. Хвостовой слеш безопасен:
`UsePathBase` срезает его сам, поэтому `/` и `/api/` валидны. Всё остальное —
ведущий слеш отсутствует, двойные слеши, посторонние символы — роняет приложение
на старте; сообщение содержит само значение и описание формата.

| Валидно | Невалидно |
| --- | --- |
| `""`, `/`, `/api`, `/api/` | `api`, `//relative`, `/api?x=1` |
| `/api/v1`, `/api/v1.0` | `/api;v=1`, `/api&x=1` |
| `/my-app`, `/my_app`, `/api~x` | `/апи`, `/%D0%B0`, `/api` + перевод строки |

Percent-encoding не поддерживается: базовый путь сравнивается с уже раскодированным
путём запроса, поэтому `%D0%B0` останется буквальными символами и никогда не совпадёт.
Не-ASCII символы тоже запрещены — технически они работают, но базовый путь попадает
в спецификацию OpenAPI, в конфигурацию обратного прокси и в логи, где становится
источником двойного кодирования.

Настройки регистрирует `AddConfiguredPathBase()`, подключает в конвейер —
`UseConfiguredPathBase()`. Без парного `Add*` второй метод падает
с `InvalidOperationException`, а не включает пустой префикс молча.

### Когда базовый путь задавать не здесь

Решение о размещении приложения под префиксом — инфраструктурное, его принимает обратный
прокси. Но исполнять это решение обязано приложение: генерация абсолютных адресов живёт
внутри — редиректы, заголовок `Location`, `servers` в спецификации OpenAPI, ссылки
Swagger UI. Приложение, не знающее своего внешнего префикса, отдаёт битые адреса
независимо от того, насколько аккуратно настроен прокси. Поэтому вопрос не в том, знать
ли о префиксе, а в том, откуда приложение о нём узнаёт.

Каналов два.

**Из собственной конфигурации** — то, что делает пара `AddConfiguredPathBase()` /
`UseConfiguredPathBase()`. Знание
дублируется: `location /api` в прокси и `PathBaseAppSettings:Path` здесь. Рассинхрон
ломает приложение тихо. Подходит, когда прокси нет вовсе (локальный запуск,
docker-compose) или когда он не умеет сообщать префикс.

**От прокси, через заголовок `X-Forwarded-Prefix`** — начиная с .NET 9 его обрабатывает
штатный `ForwardedHeadersMiddleware`, сам выставляя `Request.PathBase`:

```csharp
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedPrefix;
    // по умолчанию доверяется только loopback — в контейнерной сети список расширяется
    options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 8));
});

app.UseForwardedHeaders();
```

Единственный источник истины — конфигурация прокси. Две оговорки: nginx этот заголовок
сам не отправляет, нужен явный `proxy_set_header X-Forwarded-Prefix /api;`, и заголовку
нельзя доверять без ограничения доверенных источников — иначе клиент подделает префикс
и получит подменённые ссылки.

> Способы взаимоисключающие. Если включить оба, префикс применится дважды.

Базовый путь не заменяет основной: приложение остаётся доступным и по исходным адресам.
В спецификацию OpenAPI базовый путь автоматически не попадает — его нужно указать
в списке серверов (см. `Servers` в README пакета `AndreyAkaSkif.ServiceDefaults.Swagger`).

## Собственные настройки
Класс настроек — чистый POCO, правила проверки живут отдельно, в `IValidateOptions<T>`:

```csharp
public sealed record DemoAppSettings
{
    public string Greeting { get; init; } = string.Empty;
}

internal sealed class DemoAppSettingsValidator : IValidateOptions<DemoAppSettings>
{
    public ValidateOptionsResult Validate(string? name, DemoAppSettings options)
        => string.IsNullOrWhiteSpace(options.Greeting)
            ? ValidateOptionsResult.Fail(
                $"Требуется {nameof(DemoAppSettings)}:{nameof(DemoAppSettings.Greeting)}")
            : ValidateOptionsResult.Success;
}

builder.AddAppSettings<DemoAppSettings, DemoAppSettingsValidator>();
```

Секция конфигурации в этом случае — `DemoAppSettings`. Значение доступно из DI-контейнера
как singleton, поэтому сервис объявляет зависимость как `DemoAppSettings`, а не как
`IOptions<DemoAppSettings>`, и о конвейере параметров не знает. Рабочий образец —
`DemoAppSettings` в `samples/`.

Валидатор — обязательный параметр типа, а не перегрузка: настройки без правил валидации
зарегистрировать нельзя. Он активируется контейнером, поэтому при необходимости принимает
зависимости через конструктор. Все нарушения накапливаются в одном
`ValidateOptionsResult.Fail(...)` — конвейер покажет весь список, а не первое попавшееся.

Настройкам инфраструктуры разворачивать значение незачем: для них есть
`AddValidatedOptions<T, TValidator>()`, оставляющий в контейнере `IOptions<T>`.

Готовый объект, собранный вручную, конфигурацией не является — это обычная регистрация
`builder.Services.AddSingleton(arg)`, отдельного API она не требует.

## Настройки и fail fast
Методы `Add*`, читающие конфигурацию, регистрируют секцию в конвейере параметров
с `ValidateOnStart()`. Некорректная конфигурация роняет приложение при старте хоста,
а не отложенно — при первом запросе, которому эти настройки понадобились.

Это позже, чем `builder.Build()`, но всё ещё до первого запроса: приложение с пустой
политикой CORS или неверным адресом внешнего API не поднимется. Тип исключения —
`OptionsValidationException`, в сообщении перечислены все нарушенные правила.

Правило единое для всего пакета: `AddApiClient<TClient, TOptions>()` живёт по нему же.
Значение настроек читается один раз, при первом разрешении из контейнера, — изменение
конфигурации на лету на уже созданные объекты не влияет; за перечитыванием следует
обращаться к `IOptionsMonitor<T>`.

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
