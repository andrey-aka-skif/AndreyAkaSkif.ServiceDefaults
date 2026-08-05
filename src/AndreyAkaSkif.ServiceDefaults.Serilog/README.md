# Service Defaults Logging with Serilog

Единообразное подключение Serilog одним вызовом.

Пакет самостоятельный: зависимости от `AndreyAkaSkif.ServiceDefaults` у него нет,
он может использоваться отдельно.

## Когда нужен этот пакет

Консольный вывод в ASP.NET есть из коробки, и для контейнера этого обычно достаточно:
приложение пишет в stdout, среда исполнения его подхватывает, а дальше логи обрабатывает
сборщик — Docker, Kubernetes, Loki, ELK. В такой схеме дополнительный пакет не нужен.

Пакет добавляется, когда логи нужно писать куда-то ещё:

- в файл — приложение вне контейнера, служба Windows, отладка на месте,
  требование хранить логи рядом с приложением;
- в JSON вместо текстовых строк, чтобы логи можно было разбирать машинно;
- во внешнюю систему — Seq, Elasticsearch, база данных;
- сразу в несколько мест, каждое со своим уровнем.

Если ничего из этого не требуется, подключать пакет не стоит: он не даёт преимуществ
перед `builder.Logging.AddConsole()`, но добавляет зависимость и ещё одну секцию
конфигурации.

## Установка

```sh
dotnet add package AndreyAkaSkif.ServiceDefaults.Serilog
```

Поддерживаются `net9.0` и `net10.0`.

## Возможности
- `AddConfiguredLoggingViaSerilog()` — логгер собирается целиком из секции `Serilog`
  конфигурации приложения (`ReadFrom.Configuration`) и подключается к хосту.
- Параметр `exclusive` управляет тем, вытесняет ли Serilog остальные провайдеры
  логирования или добавляется к ним.

Куда писать, с каким уровнем, чем дополнять записи и как их форматировать — всё задаётся
в той же секции конфигурации, в коде пакета ничего из этого не зашито.
См. раздел «Файлы конфигурации».

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
приложения, поэтому буферизованные записи успевают дойти до места назначения.

## Файлы конфигурации

Куда писать логи, задаётся в секции `Serilog`: сборка указывается в `Using`, сама
настройка — в `WriteTo`. В документации Serilog каждое такое место назначения называется
*sink* — по этому слову оно и ищется.

Пример ниже настраивает запись в файл — основной сценарий, ради которого пакет
и подключают.
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

В контейнере запись в файл имеет смысл только на смонтированном томе: без него файл
останется в слое контейнера и исчезнет вместе с ним при пересоздании — то есть ровно
тогда, когда логи и нужны. Если тома нет, разумнее писать в консоль и отдать логи
сборщику.

При `rollingInterval: Day` файлы удаляются автоматически: `retainedFileCountLimit`
по умолчанию равен 31, то есть хранится последний месяц. Значение задаётся в `Args`,
`null` отключает удаление.

### Что доступно без дополнительных пакетов

`Serilog.AspNetCore` приносит транзитивно четыре сборки, поэтому для примера выше и для
вариантов ниже ставить ничего не нужно:

| Сборка                       | Что даёт                                          |
| ---------------------------- | ------------------------------------------------- |
| `Serilog.Sinks.File`         | запись в файл, имя в `WriteTo` — `File`           |
| `Serilog.Sinks.Console`      | вывод в консоль, имя в `WriteTo` — `Console`      |
| `Serilog.Sinks.Debug`        | вывод в окно отладчика, имя в `WriteTo` — `Debug` |
| `Serilog.Formatting.Compact` | форматирование записей в JSON                     |

JSON вместо текстовых строк получается из последней сборки: форматтер указывается
в `Args`, отдельный пакет не требуется.

```json
"Using": [ "Serilog.Sinks.Console" ],
"WriteTo": [
    {
        "Name": "Console",
        "Args": {
            "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
    }
]
```

Для записи куда-либо ещё — Seq, Elasticsearch, база данных — соответствующий пакет
ставится отдельно и указывается в `Using`.

## Документация
Подробности и примеры:
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults

## Сообщить о проблеме
https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/issues
