# Service Defaults OpenAPI via Swagger

Расширение для `ServiceDefaults`, добавляющее единообразную конфигурацию OpenAPI спецификации с использованием Swagger UI.

## Установка

```sh
dotnet add package AndeyAkaSkif.ServiceDefaults.Swagger
```

## Возможности
- Готовая конфигурация OpenAPI спецификации для ASP.NET Web API приложений на основе Swagger UI.

Пример
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultOpenApiViaSwagger();

var app = builder.Build();
app.UseDefaultOpenApiViaSwagger();

app.Run();
```

## Документация
Подробности и примеры:
https://github.com/andrey-aka-skif/ServiceDefaults
