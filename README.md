# <img alt="logo" src="./logo/logo.png" width="32"/> Базовая конфигурация WEB-API сервисов ASP.NET

[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/LICENSE)
[![CI](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/ci.yml/badge.svg)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/ci.yml)
[![NuGet Publish](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/package.yml/badge.svg)](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/actions/workflows/package.yml)

Набор вспомогательных библиотек для упрощённой конфигурации ASP.NET Web-API сервисов.  
Проект предоставляет набор классов и методов расширения для настройки:

- минимальных Web API сервисов,
- валидации,
- логирования,
- стандартного поведения приложения,
- middleware обработки ошибок,
- политик CORS,
- http-клиентов,
- OpenAPI (Swagger),
- Problems Details,
- роутинга API,
- Health Checks,
- локализации,
- интеграции с Serilog (опционально).

Идея вдохновлена проектом [eShop.ServiceDefaults](https://github.com/dotnet/eShop/tree/main/src/eShop.ServiceDefaults) из [eShop Reference Application](https://github.com/dotnet/eShop).

## 📦 Состав пакетов

Репозиторий содержит два NuGet-пакета:

### 1. **ServiceDefaults**
Базовые методы расширения для конфигурирования ASP.NET Web-API сервисов:  
- логирование,
...

👉 Подробнее: `src/ServiceDefaults/README.md`

---

### 2. **ServiceDefaults.Serilog**
Расширение `ServiceDefaults` для подключения Serilog:  
- единая конфигурация Serilog.

👉 Подробнее: `src/ServiceDefaults.Serilog/README.md`

---

## 📚 Документация и примеры

Каждый пакет содержит свой отдельный README с:

- примером установки,
- минимальными примерами интеграции,
- ссылками на дополнительные материалы.

👉 Запускаемый пример сервиса, собранного на этих пакетах: [`samples/README.md`](./samples/README.md)

---

## 📄 Лицензия

[MIT](./LICENSE)
