# samples — демонстрационный сервис

Запускаемый Web-API, собранный на пакетах этого репозитория. Нужен, чтобы публичный
API библиотеки можно было проверить руками, а ломающие изменения ловились сборкой,
а не только юнит-тестами.

Пакеты подключены через `ProjectReference` на `src/`: пример всегда собирается против
текущего исходника, опубликованные версии и токен к GitHub Packages не требуются.

## Запуск

Нужен .NET SDK 10. Из корня репозитория:

```bash
dotnet run --project samples/src/AndreyAkaSkif.ServiceDefaults.Samples.Api
```

Или откройте `samples/AndreyAkaSkif.ServiceDefaults.Samples.slnx` в IDE и запустите
профиль `http` (`http://localhost:5080`) либо `https` (`https://localhost:5081`).

## Что посмотреть

| Адрес                         | Что демонстрирует                                                                                                                                           |
| ----------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `/`                           | Редирект на Swagger UI (`UseConfiguredOpenApiViaSwagger`)                                                                                                   |
| `/swagger`                    | Swagger UI, только в Development                                                                                                                            |
| `/swagger/1.0.0/swagger.json` | Спецификация из секции `SwaggerAppSettings`                                                                                                                 |
| `/health`                     | `MapHealthCheckEndpoint`; в спецификацию точку добавляет `AddHealthCheckEndpointWithSwagger`                                                                |
| `/api/health`                 | Тот же обработчик через базовый путь из `PathBaseAppSettings` (`UseConfiguredPathBase`)                                                                     |
| `/demo/greeting`              | Настройки `DemoAppSettings`, провалидированные до `Build()` и взятые из DI                                                                                  |
| `/demo/greeting/Мир`          | Сервис с собственным объектом-параметром `GreetingServiceArgs` вместо объекта секции                                                                        |
| `/demo/echo?message=hi`       | Обычный маршрут minimal API                                                                                                                                 |
| `/demo/channel/CurrentA`      | Перечисление как сегмент пути (`AddEnumRouteConstraint<DemoChannel>`). `currenta` и `0` тоже работают и приводятся к `CurrentA`, `999` и `unknown` дают 404 |
| `/demo/boom`                  | `ProblemDetails` от `AddExtendedErrorHandling`: в Development с полем `exception`                                                                           |

Логи пишет Serilog (`AddConfiguredLoggingViaSerilog`), конфигурация — секция `Serilog`.
Логи идут в консоль, чтобы пример запускался одной командой и не оставлял после себя
файлов. Ради консоли отдельный пакет не нужен — её умеет и встроенный провайдер; Serilog
подключают, когда логи нужно писать куда-то ещё, см. README пакета.
Политика CORS берётся из секции `CorsPolicy`.

## Как разложен код

```
Program.cs              подключение пакетов библиотеки и конвейер
AppConfiguration/       конфигурация, специфичная для приложения
AppSettings/            объекты секций конфигурации
Endpoints/              конечные точки и типы, которые в них участвуют
Services/               сервисы приложения и их объекты-параметры
```

В `AppConfiguration/` лежат четыре метода расширения — по одному на тему, чтобы
`Program.cs` не рос по мере развития приложения:

| Метод                      | Что регистрирует                                                      |
| -------------------------- | --------------------------------------------------------------------- |
| `AddAppSettings()`         | объекты настроек, здесь — `DemoAppSettings`                           |
| `AddAppDbContexts()`       | контексты EF Core; в образце выключен, требует запущенного PostgreSQL |
| `AddAppServices()`         | сервисы приложения, здесь — `GreetingService`                         |
| `AddAppRouteConstraints()` | ограничения параметров маршрута, здесь — `DemoChannel`                |

### Почему объекты настроек лежат в двух разных местах

За словом «настройки» скрываются две роли, и образец показывает обе.

`DemoAppSettings` — **объект секции конфигурации**. Он знает имя секции (оно равно имени
типа), реализует `IValidatableSettingsObject` и читается из `appsettings.json`. Это тип
границы приложения, поэтому он лежит в `AppSettings/` и регистрируется через
`AddServiceArgFromValidatedSettingsObject<T>()`.

`GreetingServiceArgs` — **объект-параметр сервиса**. Про конфигурацию он не знает ничего
и наследовать `IValidatableSettingsObject` не обязан; он существует ровно затем, чтобы
в домен не пришлось тащить `IOptions<T>`. Такой тип принадлежит сервису и лежит рядом
с ним, в `Services/`.

Переход между ролями происходит в composition root — в `AddAppServices()`, где значение
из `DemoAppSettings` отображается в `GreetingServiceArgs`. Сам сервис остаётся
независимым от того, как приложение читает настройки. Разницу видно на паре соседних
конечных точек: `/demo/greeting` работает с объектом секции, `/demo/greeting/{name}` —
с сервисом и его собственным аргументом.

Вызовы пакетов — `AddConfiguredCorsPolicy()`, `AddConfiguredOpenApiViaSwagger()`
и остальные — в `AppConfiguration/` **не** заворачивались и остались в `Program.cs`
на виду. Обёртка сократила бы `Program.cs`, но спрятала бы ровно то, ради чего образец
и существует: чтобы увидеть, как подключается CORS, пришлось бы открывать другой файл.

## Почему адреса в `SwaggerAppSettings.Servers` относительные

В примере это `"/"` и `"/api"`, а не абсолютные URL. Swagger UI берёт базовый адрес
запросов из `servers[0]` спецификации, поэтому абсолютный адрес ломает всё, что открыто
не по нему: с профилем `https` (страница на `https://localhost:5081`) запросы ушли бы на
`http://localhost:5080` и браузер отрезал бы их как mixed content — «Failed to fetch»
на каждой конечной точке. То же случилось бы за reverse proxy или на другом хосте.

Относительный адрес OpenAPI 3 допускает, и UI резолвит его от origin страницы: работают
оба профиля, а запрос остаётся same-origin, то есть CORS вообще не участвует.

Список необязателен: при пустом или отсутствующем `Servers` пакет подставляет `"/"`.
В примере он задан явно, чтобы был виден и сам параметр, и формат адресов.

## Что закомментировано

- **`AndreyAkaSkif.ServiceDefaults.OpenApi`** — альтернатива пакету `.Swagger`: спецификация
  средствами ASP.NET, без Swagger UI. Одновременно с `.Swagger` в примере не показывается.
- **`AndreyAkaSkif.ServiceDefaults.PostgreSQL`** — требует запущенного PostgreSQL, поэтому
  выключен: иначе пример перестал бы запускаться одной командой.

В обоих случаях нужно раскомментировать `ProjectReference` в
[csproj](src/AndreyAkaSkif.ServiceDefaults.Samples.Api/AndreyAkaSkif.ServiceDefaults.Samples.Api.csproj)
и соответствующий код вместе с его `using`-ами:

- для OpenApi — блок в [Program.cs](src/AndreyAkaSkif.ServiceDefaults.Samples.Api/Program.cs),
  это выбор пакета библиотеки, а не прикладная настройка;
- для PostgreSQL — тело метода и класс `DemoDbContext`
  в [AppDbContextsConfigureExtensions.cs](src/AndreyAkaSkif.ServiceDefaults.Samples.Api/AppConfiguration/AppDbContextsConfigureExtensions.cs),
  плюс секцию `ConnectionStrings` в `appsettings.Development.json`.

Оба варианта компилируются.

## Solution

У примера свой `samples/AndreyAkaSkif.ServiceDefaults.Samples.slnx`: корневой solution
остаётся только для библиотеки и тестов. В solution примера дополнительно включены все
пять проектов из `src/` — для навигации по исходнику из IDE.

CI собирает `samples/*.slnx` отдельным шагом, поэтому пример не может незаметно
разойтись с библиотекой.
