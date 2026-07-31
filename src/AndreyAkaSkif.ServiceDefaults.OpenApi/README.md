# Service Defaults OpenAPI

Расширение для `ServiceDefaults`, добавляющее единообразную конфигурацию OpenAPI спецификации.

## Установка

```sh
dotnet add package AndeyAkaSkif.ServiceDefaults.OpenApi
```

## Возможности
- Готовая конфигурация OpenAPI спецификации для ASP.NET Core Web API приложений.

Пример
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultOpenApi();

var app = builder.Build();
app.UseDefaultOpenApi();

app.Run();
```

## Документация
Подробности и примеры:
https://github.com/andrey-aka-skif/ServiceDefaults
