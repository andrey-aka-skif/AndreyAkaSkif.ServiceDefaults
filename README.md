# <img alt="logo" src="./logo/logo.png" width="32"/> Базовая конфигурация WEB-API сервисов ASP.NET

[![CI](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/ci.yml/badge.svg)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/ci.yml)
[![Release](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/release.yml/badge.svg)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/release.yml)
[![GitHub license](https://img.shields.io/github/license/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults.svg)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/LICENSE)
[![Docs](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/docs.yml/badge.svg)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/docs.yml)

Набор вспомогательных библиотек для упрощённой конфигурации ASP.NET Web-API сервисов.  
Проект предоставляет методы расширения для настройки:

- политик CORS — настраиваемой через конфигурацию и разрешительной,
- обработки ошибок через `ProblemDetails`,
- базового пути API (PathBase),
- ограничений параметров маршрута, включая перечисление как сегмент пути,
- конечной точки проверки жизнеспособности (`/health`),
- объектов настроек с обязательной валидацией, проверяемых до первого запроса,
- типизированных API-клиентов к внешним REST-сервисам,
- спецификации OpenAPI — без UI и через Swagger UI,
- контекста PostgreSQL на Entity Framework Core,
- логирования через Serilog.

Идея вдохновлена проектом [eShop.ServiceDefaults](https://github.com/dotnet/eShop/tree/main/src/eShop.ServiceDefaults) из [eShop Reference Application](https://github.com/dotnet/eShop).

## Состав пакетов

Репозиторий содержит пять NuGet-пакетов. Базовым является `AndreyAkaSkif.ServiceDefaults`;
остальные подключаются по мере надобности.

| Пакет                                      | Назначение                                                                                                  | README                                                             |
| ------------------------------------------ | ----------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------ |
| `AndreyAkaSkif.ServiceDefaults`            | CORS, обработка ошибок, PathBase, ограничения маршрутов, Health Checks, настройки с валидацией, API-клиенты | [README](./src/AndreyAkaSkif.ServiceDefaults/README.md)            |
| `AndreyAkaSkif.ServiceDefaults.OpenApi`    | Спецификация OpenAPI средствами ASP.NET, без UI                                                             | [README](./src/AndreyAkaSkif.ServiceDefaults.OpenApi/README.md)    |
| `AndreyAkaSkif.ServiceDefaults.Swagger`    | Спецификация OpenAPI и Swagger UI                                                                           | [README](./src/AndreyAkaSkif.ServiceDefaults.Swagger/README.md)    |
| `AndreyAkaSkif.ServiceDefaults.PostgreSQL` | Простой контекст PostgreSQL на EF Core                                                                      | [README](./src/AndreyAkaSkif.ServiceDefaults.PostgreSQL/README.md) |
| `AndreyAkaSkif.ServiceDefaults.Serilog`    | Логирование через Serilog: запись в файл, в JSON и во внешние системы                                       | [README](./src/AndreyAkaSkif.ServiceDefaults.Serilog/README.md)    |

Зависимость от базового пакета есть только у `.Swagger`: он описывает в спецификации
конечную точку `/health` и берёт её адрес из константы `HealthCheckDefaults.Endpoint`.
Остальные пакеты ставятся независимо друг от друга.

---

## Документация и примеры

Каждый пакет содержит свой отдельный README с:

- примером установки,
- минимальными примерами интеграции,
- описанием секций конфигурации.

Запускаемый пример сервиса, собранного на этих пакетах: [`samples/README.md`](./samples/README.md)

---

## Просмотр документации

Документация проекта создана с помощью инструмента [DocFX](https://github.com/dotnet/docfx).
Сгенерированная документация расположена на сервисе
[github.io](https://andrey-aka-skif.github.io/AndreyAkaSkif.ServiceDefaults/): те же статьи
плюс справочник API, собранный из XML-комментариев.

Для просмотра локальной документации использовать команды
(docfx подключён как локальный инструмент):

```shell
dotnet tool restore
dotnet docfx docs/docfx.json --serve
```

---

## Лицензия

[MIT](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/LICENSE)
