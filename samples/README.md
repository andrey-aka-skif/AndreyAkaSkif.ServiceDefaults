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

| Адрес | Что демонстрирует |
| --- | --- |
| `/` | Редирект на Swagger UI (`UseConfiguredOpenApiViaSwagger`) |
| `/swagger` | Swagger UI, только в Development |
| `/swagger/1.0.0/swagger.json` | Спецификация из секции `SwaggerAppSettings` |
| `/health` | `MapHealthCheckEndpoint`; в спецификацию точку добавляет `AddHealthCheckEndpointWithSwagger` |
| `/api/health` | Тот же обработчик через базовый путь из `RouteAppSettings` (`UseConfiguredPathBase`) |
| `/demo/greeting` | Настройки `DemoAppSettings`, провалидированные до `Build()` и взятые из DI |
| `/demo/echo?message=hi` | Обычный маршрут minimal API |
| `/demo/boom` | `ProblemDetails` от `AddExtendedErrorHandling`: в Development с полем `exception` |

Логи пишет Serilog (`AddConfiguredLoggingViaSerilog`), конфигурация — секция `Serilog`.
Политика CORS берётся из секции `CorsPolicy`.

## Почему адреса в `SwaggerAppSettings.Servers` относительные

В примере это `"/"` и `"/api"`, а не абсолютные URL. Swagger UI берёт базовый адрес
запросов из `servers[0]` спецификации, поэтому абсолютный адрес ломает всё, что открыто
не по нему: с профилем `https` (страница на `https://localhost:5081`) запросы ушли бы на
`http://localhost:5080` и браузер отрезал бы их как mixed content — «Failed to fetch»
на каждой конечной точке. То же случилось бы за reverse proxy или на другом хосте.

Относительный адрес OpenAPI 3 допускает, и UI резолвит его от origin страницы: работают
оба профиля, а запрос остаётся same-origin, то есть CORS вообще не участвует.

## Два обязательных вызова рядом с библиотекой

В `Program.cs` есть два вызова, которые пакеты за приложение не делают:

- `builder.Services.AddEndpointsApiExplorer()` — без него `AddConfiguredOpenApiViaSwagger`
  падает на старте: `SwaggerGenerator` строится поверх ApiExplorer, а для minimal API
  тот не регистрируется сам;
- `builder.Logging.ClearProviders()` — `AddConfiguredLoggingViaSerilog` добавляет Serilog
  к уже настроенным провайдерам, поэтому без очистки каждая запись уходит в консоль дважды.

## Что закомментировано

- **`AndreyAkaSkif.ServiceDefaults.OpenApi`** — альтернатива пакету `.Swagger`: спецификация
  средствами ASP.NET, без Swagger UI. Одновременно с `.Swagger` в примере не показывается.
- **`AndreyAkaSkif.ServiceDefaults.PostgreSQL`** — требует запущенного PostgreSQL, поэтому
  выключен: иначе пример перестал бы запускаться одной командой.

Чтобы включить любой из блоков, нужно раскомментировать `ProjectReference` в
[csproj](src/AndreyAkaSkif.ServiceDefaults.Samples.Api/AndreyAkaSkif.ServiceDefaults.Samples.Api.csproj),
блок в [Program.cs](src/AndreyAkaSkif.ServiceDefaults.Samples.Api/Program.cs) вместе с его
`using`-ами, а для PostgreSQL — ещё и секцию `ConnectionStrings` в
`appsettings.Development.json`. Оба варианта компилируются.

## Solution

У примера свой `samples/AndreyAkaSkif.ServiceDefaults.Samples.slnx`: корневой solution
остаётся только для библиотеки и тестов. В solution примера дополнительно включены все
пять проектов из `src/` — для навигации по исходнику из IDE.

CI собирает `samples/*.slnx` отдельным шагом, поэтому пример не может незаметно
разойтись с библиотекой.
