# Service Defaults OpenAPI

Единообразная конфигурация спецификации OpenAPI для ASP.NET Web-API приложений.

Пакет самостоятельный: зависимости от `AndreyAkaSkif.ServiceDefaults` у него нет,
он может использоваться отдельно.

## Установка

```sh
dotnet add package AndreyAkaSkif.ServiceDefaults.OpenApi
```

## Возможности
- Готовая конфигурация спецификации OpenAPI средствами ASP.NET (`AddOpenApi()`)
  через пару методов `AddDefaultOpenApi()` / `UseDefaultOpenApi()`.

## Пример
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultOpenApi();

var app = builder.Build();
app.UseDefaultOpenApi();

app.Run();
```

## Особенности
### UI не входит в пакет
Пакет формирует только JSON спецификации, она доступна по адресу `/openapi/v1.json`.
Если нужен Swagger UI, используйте пакет `AndreyAkaSkif.ServiceDefaults.Swagger` —
одновременно с этим пакетом он не применяется.

### Только Development
`UseDefaultOpenApi()` регистрирует конечную точку спецификации **только в среде Development**.
В Production и любой другой среде конечная точка не формируется и запрос к ней вернёт
`404 Not Found`.

## Документация
Подробности и примеры:
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults

## Сообщить о проблеме
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/issues
