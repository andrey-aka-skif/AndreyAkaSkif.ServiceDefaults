# <img alt="logo" src="./logo/logo.png" width="32"/> Базовая конфигурация WEB-API сервисов ASP.NET

[![GitHub license](https://img.shields.io/github/license/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults.svg)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/LICENSE)
[![CI](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/ci.yml/badge.svg)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/ci.yml)
[![Release](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/release.yml/badge.svg)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/release.yml)

Набор вспомогательных библиотек для упрощённой конфигурации ASP.NET Web-API сервисов.  
Проект предоставляет методы расширения для настройки:

- политик CORS — настраиваемой через конфигурацию и разрешительной,
- обработки ошибок через `ProblemDetails`,
- базового пути API (PathBase),
- конечной точки проверки жизнеспособности (`/health`),
- валидируемых объектов настроек, проверяемых до старта приложения,
- спецификации OpenAPI — без UI и через Swagger UI,
- контекста PostgreSQL на Entity Framework Core,
- логирования через Serilog.

Идея вдохновлена проектом [eShop.ServiceDefaults](https://github.com/dotnet/eShop/tree/main/src/eShop.ServiceDefaults) из [eShop Reference Application](https://github.com/dotnet/eShop).

## 📦 Состав пакетов

Репозиторий содержит пять NuGet-пакетов. Базовым является `AndreyAkaSkif.ServiceDefaults`;
остальные подключаются по мере надобности.

| Пакет | Назначение | README |
| --- | --- | --- |
| `AndreyAkaSkif.ServiceDefaults` | CORS, обработка ошибок, PathBase, Health Checks, валидируемые настройки | [README](./src/AndreyAkaSkif.ServiceDefaults/README.md) |
| `AndreyAkaSkif.ServiceDefaults.OpenApi` | Спецификация OpenAPI средствами ASP.NET, без UI | [README](./src/AndreyAkaSkif.ServiceDefaults.OpenApi/README.md) |
| `AndreyAkaSkif.ServiceDefaults.Swagger` | Спецификация OpenAPI и Swagger UI | [README](./src/AndreyAkaSkif.ServiceDefaults.Swagger/README.md) |
| `AndreyAkaSkif.ServiceDefaults.PostgreSQL` | Простой контекст PostgreSQL на EF Core | [README](./src/AndreyAkaSkif.ServiceDefaults.PostgreSQL/README.md) |
| `AndreyAkaSkif.ServiceDefaults.Serilog` | Логирование через Serilog | [README](./src/AndreyAkaSkif.ServiceDefaults.Serilog/README.md) |

Зависимость от базового пакета есть только у `.Swagger`: он описывает в спецификации
конечную точку `/health` и берёт её адрес из константы `HealthCheckDefaults.Endpoint`.
Остальные пакеты ставятся независимо друг от друга.

---

## 📚 Документация и примеры

Каждый пакет содержит свой отдельный README с:

- примером установки,
- минимальными примерами интеграции,
- описанием секций конфигурации.

👉 Запускаемый пример сервиса, собранного на этих пакетах: [`samples/README.md`](./samples/README.md)

---

## 🚧 В планах

- [конфигурация http-клиентов](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/issues/6),
- [пользовательские Constraints](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/issues/29),
- [документация DocFX](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/issues/22).

---

## 🐞 Сообщить о проблеме

https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/issues

---

## 📄 Лицензия

[MIT](./LICENSE)
