# Service Defaults Logging with Serilog

Единообразное подключение Serilog одним вызовом.

Пакет самостоятельный: зависимости от `AndreyAkaSkif.ServiceDefaults` у него нет,
он может использоваться отдельно.

## Установка

```sh
dotnet add package AndreyAkaSkif.ServiceDefaults.Serilog
```

## Возможности
- `AddConfiguredLoggingViaSerilog()` — логгер собирается целиком из секции `Serilog`
  конфигурации приложения (`ReadFrom.Configuration`) и подключается к хосту.
- Параметр `exclusive` управляет тем, вытесняет ли Serilog остальные провайдеры
  логирования или добавляется к ним.

Стоки, уровни, enricher'ы и шаблоны вывода задаются в той же секции конфигурации —
в коде пакета ничего из этого не зашито. См. раздел «Файлы конфигурации».

## Пример
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguredLoggingViaSerilog(); // включает Serilog

var app = builder.Build();

app.Run();
```

## Логирование запросов
Пакет не вызывает `UseSerilogRequestLogging()`. Если нужен единый лог входящих HTTP-запросов
вместо стандартных записей ASP.NET, приложение подключает его само:

```csharp
var app = builder.Build();
app.UseSerilogRequestLogging();
```

## Монопольная установка

По умолчанию (`exclusive: true`) Serilog устанавливается монопольно: подменяется `ILoggerFactory`,
поэтому провайдеры по умолчанию, включая консольный, перестают писать и каждая запись попадает
в вывод один раз. Вызывать `builder.Logging.ClearProviders()` не требуется.
Дополнительно в DI-контейнере регистрируются `Serilog.ILogger` и `IDiagnosticContext`.

> [!WARNING]
> При монопольной установке молча перестают работать и провайдеры, добавленные самим
> приложением через `builder.Logging`: заменяется фабрика целиком, а не список провайдеров.

Если Serilog нужен рядом с другими провайдерами, следует передать `exclusive: false` —
тогда Serilog добавляется к уже настроенным провайдерам, а очистку лишних выполняет приложение:

```csharp
builder.Logging.ClearProviders();
builder.AddConfiguredLoggingViaSerilog(exclusive: false);
builder.Logging.AddConsole();
```

Время жизни логгера в обоих случаях передаётся хосту: логгер закрывается при завершении
приложения, поэтому буферизующие стоки успевают сбросить накопленные записи.

## Файлы конфигурации

Для настройки Serilog можно использовать файл `appsettings.json` с примером ниже:
```json
"Serilog": {
        "Using": [ "Serilog.Sinks.File" ],
        "MinimumLevel": {
            "Default": "Information",
            "Override": {
                "Microsoft.AspNetCore": "Warning",
                "Microsoft.Hosting.Lifetime": "Information",
                "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
            }
        },
        "WriteTo": [
            {
                "Name": "File",
                "Args": {
                    "path": "../../logs/app-api-.txt",
                    "rollingInterval": "Day",
                    "outputTemplate": "[{Timestamp:HH:mm:ss}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
                }
            }
        ],
        "Enrich": [ "FromLogContext" ]
    },
```

> [!WARNING]
> Обратить внимание, что при такой конфигурации логирование выполняется в файл `app-api-.txt`.
> Путь к файлу - относительный и зависит от текущей рабочей директории приложения
> и должен быть доступен для записи.

## Документация
Подробности и примеры:
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults

## Сообщить о проблеме
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/issues
