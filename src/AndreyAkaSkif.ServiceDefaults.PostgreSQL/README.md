# Service Defaults simple PostgreSQL Context

Регистрация контекста Entity Framework Core с провайдером PostgreSQL одним вызовом.

Пакет самостоятельный: зависимости от `AndreyAkaSkif.ServiceDefaults` у него нет,
он может использоваться отдельно.

## Установка

```sh
dotnet add package AndreyAkaSkif.ServiceDefaults.PostgreSQL
```

Поддерживаются `net9.0` и `net10.0`. На `net9.0` подтягивается
`Npgsql.EntityFrameworkCore.PostgreSQL` 9.x (и, соответственно, EF Core 9),
на `net10.0` — 10.x.

## Возможности
- `AddSimplePostgreSQLContext<T>()` — регистрация контекста, производного от `DbContext`,
  через стандартный `AddDbContext<T>()` с провайдером Npgsql и строкой подключения
  из конфигурации.

## Пример
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddSimplePostgreSQLContext<AppContext>();

var app = builder.Build();

app.Run();
```

## Конфигурация
Строка подключения берётся из раздела `ConnectionStrings` под именем `DefaultConnection`.
Имя не конфигурируется:

```json
"ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=mydb;Username=user;Password=pass"
}
```

## Особенности
### Логирование чувствительных данных
`EnableSensitiveDataLogging` включается автоматически и **только в среде Development**.
В остальных средах SQL-запросы с параметрами в логи не попадают.

### Что метод не делает
Пакет только регистрирует контекст. Миграции, `EnsureCreated()` и проверка доступности
базы данных остаются за приложением. Проверку БД, если она нужна, следует добавлять
отдельной конечной точкой Health Checks, а не в `/health` из пакета
`AndreyAkaSkif.ServiceDefaults`.

## Документация
Подробности и примеры:
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults

## Сообщить о проблеме
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/issues
