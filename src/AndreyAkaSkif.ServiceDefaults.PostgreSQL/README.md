# Service Defaults simple PostgreSQL Context

Расширение для `ServiceDefaults`, добавляющее простой контекст PostgreSQL.

## Установка

```sh
dotnet add package AndeyAkaSkif.ServiceDefaults.PostgreSQL
```

## Возможности
- PostgreSQL

Пример
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddSimplePostgreSQLContext<AppContext>();

var app = builder.Build();

app.Run();
```

## Документация
Подробности и примеры:
https://github.com/andrey-aka-skif/ServiceDefaults
