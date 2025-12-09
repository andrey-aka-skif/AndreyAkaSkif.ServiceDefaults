# ServiceDefaults

Библиотека с базовыми настройками для ASP.NET Web-API приложений.  
Предоставляет единые методы расширения для конфигурирования типового сервиса.

## Установка

```sh
dotnet add package AndeyAkaSkif.ServiceDefaults
```

## Возможности
- Стандартная конфигурация минимального API
- Обработка ошибок
- OpenAPI/Swagger
- Health Checks
- Logging Abstractions
- Observability и метрические endpoints
- Единый пайплайн для всех сервисов в решении

Пример
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();  // добавляет стандартные сервисы

var app = builder.Build();
app.MapDefaultEndpoints();     // подключает стандартный набор endpoint'ов

app.Run();
```

## Документация
Полное описание и примеры:
https://github.com/andrey-aka-skif/ServiceDefaults
