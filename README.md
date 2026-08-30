# <img alt="logo" src="./logo/logo.png" width="32"/> Базовая конфигурация WEB-API сервисов ASP.NET

[![License](https://img.shields.io/github/license/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults.svg?label=License)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/LICENSE)
[![CI](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/ci.yml/badge.svg)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/ci.yml)
[![Publish](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/publish.yml/badge.svg)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/publish.yml)
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
- спецификации OpenAPI — генерация встроенными средствами ASP.NET и показ через Swagger UI,
- контекста PostgreSQL на Entity Framework Core,
- логирования через Serilog.

Идея вдохновлена проектом [eShop.ServiceDefaults](https://github.com/dotnet/eShop/tree/main/src/eShop.ServiceDefaults) из [eShop Reference Application](https://github.com/dotnet/eShop).

## Состав пакетов

Репозиторий содержит пять NuGet-пакетов. Базовым является `AndreyAkaSkif.ServiceDefaults`;
остальные подключаются по мере надобности.

| Пакет                                      | Назначение                                                                                                  | README                                                             |
| ------------------------------------------ | ----------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------ |
| `AndreyAkaSkif.ServiceDefaults`            | CORS, обработка ошибок, PathBase, ограничения маршрутов, Health Checks, настройки с валидацией, API-клиенты | [README](./src/AndreyAkaSkif.ServiceDefaults/README.md)            |
| `AndreyAkaSkif.ServiceDefaults.OpenApi`    | Спецификация OpenAPI средствами ASP.NET: атрибуция, доступность, `/health`                                  | [README](./src/AndreyAkaSkif.ServiceDefaults.OpenApi/README.md)    |
| `AndreyAkaSkif.ServiceDefaults.Swagger`    | Показ спецификации OpenAPI через Swagger UI                                                                 | [README](./src/AndreyAkaSkif.ServiceDefaults.Swagger/README.md)    |
| `AndreyAkaSkif.ServiceDefaults.PostgreSQL` | Простой контекст PostgreSQL на EF Core                                                                      | [README](./src/AndreyAkaSkif.ServiceDefaults.PostgreSQL/README.md) |
| `AndreyAkaSkif.ServiceDefaults.Serilog`    | Логирование через Serilog: запись в файл, в JSON и во внешние системы                                       | [README](./src/AndreyAkaSkif.ServiceDefaults.Serilog/README.md)    |

Зависимостей между пакетами нет: каждый ставится сам по себе. `.OpenApi` и `.Swagger`
делят между собой одну задачу — первый спецификацию генерирует, второй показывает, —
но знают друг о друге только через адрес документа в конфигурации, поэтому применяются
и вместе, и порознь.

---

## Поддерживаемые фреймворки

Все пакеты собираются под `net9.0` и `net10.0`. Приложению достаточно любой из
этих версий, переезд на .NET 10 ради подключения библиотеки не требуется.

Зависимости на ветке `net9.0` разрешаются в свои мажоры: `.OpenApi` тянет
`Microsoft.AspNetCore.OpenApi` 9.x вместо 10.x, `.PostgreSQL` —
`Npgsql.EntityFrameworkCore.PostgreSQL` 9.x вместо 10.x. Публичный API пакетов
на обеих ветках одинаков.

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

## Разработка

Внутреннее устройство репозитория — сборка, проверки, порядок выпуска и токены
реестров: [CONTRIBUTING.md](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/CONTRIBUTING.md)

---

## Лицензия

[MIT](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/LICENSE)
