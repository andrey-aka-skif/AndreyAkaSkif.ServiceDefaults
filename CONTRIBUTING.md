# Разработка

Внутреннее устройство репозитория: как собираются пакеты, чем проверяются и как
попадают в реестры. Всё, что касается использования библиотек, — в
[документации](https://andrey-aka-skif.github.io/AndreyAkaSkif.ServiceDefaults/)
и в README каждого пакета.

Файл лежит в корне репозитория и подключён разделом «Разработка» на сайте
документации, поэтому ссылки в нём держатся абсолютными: относительные не
разрешатся при сборке сайта.

## Требования

.NET SDK 10. Рантайм 9 тоже нужен: пакеты собираются под `net10.0` и `net9.0`
(мультитаргетинг задан в
[Directory.Build.props](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/Directory.Build.props)),
а `dotnet test` поднимает тестовые сборки каждая на своём рантайме. Собрать
`net9.0` SDK десятый умеет, а выполнить — нет.

```shell
dotnet restore
```

## Структура

```
src/AndreyAkaSkif.ServiceDefaults/             базовый пакет
src/AndreyAkaSkif.ServiceDefaults.OpenApi/     спецификация OpenAPI без UI
src/AndreyAkaSkif.ServiceDefaults.Swagger/     спецификация OpenAPI и Swagger UI
src/AndreyAkaSkif.ServiceDefaults.PostgreSQL/  контекст PostgreSQL на EF Core
src/AndreyAkaSkif.ServiceDefaults.Serilog/     логирование через Serilog
tests/                                         тесты, xUnit v3
samples/                                       пример сервиса, отдельное решение
docs/                                          исходники сайта документации, DocFX
.ci/actions/                                   композитные экшены, общие для GitHub и Gitea
```

Упаковываемых проектов пять, и выпускаются они одной версией: держать пять линий
версий ради независимых пакетов — лишняя работа, а зависимость между ними всё
равно есть (`.Swagger` берёт адрес `/health` из базового пакета).

Пример сервиса — отдельное решение
([samples/AndreyAkaSkif.ServiceDefaults.Samples.slnx](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/samples/AndreyAkaSkif.ServiceDefaults.Samples.slnx)),
корневой `dotnet restore` его не видит. В CI он собирается отдельным шагом
намеренно: ломающие изменения публичного API ловятся именно там.

## Сборка и тесты

```shell
dotnet build --configuration Release
```

```shell
dotnet test --configuration Release
```

Тесты прогоняются для обоих таргетов. Публичный API на `net9.0` и `net10.0`
одинаков, поэтому расхождение между таргетами — это дефект, а не ожидаемое
поведение. Различаются только версии зависимостей: на ветке `net9.0` они
разрешаются в свои мажоры (`Microsoft.AspNetCore.OpenApi` 9.x,
`Npgsql.EntityFrameworkCore.PostgreSQL` 9.x), условные по TFM `PackageVersion`
заданы в
[Directory.Packages.props](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/Directory.Packages.props).

Пример сервиса собирается своей командой:

```shell
dotnet build samples/AndreyAkaSkif.ServiceDefaults.Samples.slnx --configuration Release
```

## Документация

DocFX, версия закреплена в
[.config/dotnet-tools.json](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/.config/dotnet-tools.json).

```shell
dotnet tool restore
```

```shell
dotnet docfx docs/docfx.json --serve
```

Статьи сайта — это те же README, отдельных копий нет: они подтягиваются в сайт
записью `content` из
[docs/docfx.json](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/docs/docfx.json),
а навигация задаётся в
[docs/articles/toc.yml](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/docs/articles/toc.yml).
Этот файл попадает туда же.

Сайт выкладывается на GitHub Pages воркфлоу
[docs.yml](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/.github/workflows/docs.yml)
на пуш в `master` — отдельно от релиза пакетов, чтобы правки статей доезжали до
читателя, не дожидаясь выпуска. Сборка docfx входит и в CI, последними шагами:
битые ссылки и невалидный `toc.yml` роняют шаг до мерджа, а не после. Шаги стоят
последними намеренно — сборка, тесты и упаковка к тому моменту уже отработали, и
падение документации их результат не отменяет.

Каталоги `docs/_site/` и `docs/api/` в репозиторий не попадают.

## Версии и публикация

Версия в исходниках не хранится. Её проставляет CI из git-тега вида `v1.2.3`,
передавая в `dotnet build` и `dotnet pack` через `-p:Version=`. Разбор и
проверка тега — в общем экшене
[.ci/actions/prepare-nuget-release](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/.ci/actions/prepare-nuget-release/action.yml),
одном на оба зеркала: формат тега обязан совпадать, поэтому правило живёт в
одном экземпляре и требует ведущего `v`.

Публикация запускается **публикацией релиза**, а не push тега: релиз — это
преднамеренный акт с release notes. Ручного запуска у воркфлоу нет. Упавший
прогон переигрывается кнопкой Re-run, невыпущенный тег добирается созданием
релиза, а переиздать уже выпущенную версию нельзя вовсе — реестр не даёт
перезаписать пакет той же версии.

Помимо GitHub в репозитории лежит зеркальный набор воркфлоу для Gitea
([.gitea/workflows](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/tree/master/.gitea/workflows)).
Gitea тянет с GitHub только рефы, релизы у неё свои и создаются отдельно. Сайт
документации собирается и выкладывается только на стороне GitHub — Pages у
Gitea нет.

Версии экшенов на GitHub поднимает dependabot
([dependabot.yml](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/.github/dependabot.yml)).
Каталог `.gitea/workflows` он не видит: там версии правятся вручную вслед за
GitHub-стороной.

### Почему задания в Gitea идут на node-образе

Метка `node-24` в `runs-on` выглядит нелепо для .NET-проекта, но выбрана
осознанно. `act_runner` исполняет JavaScript-экшены — `actions/checkout`,
`actions/cache`, `actions/setup-dotnet` — внутри контейнера задания, запуская
их системным `node`. Образ без ноды роняет первый же шаг:

```
exec: "node": executable file not found in $PATH
```

Метка `dotnet-10` у раннера есть, но для воркфлоу с JS-экшенами непригодна:
ноды в образе .NET SDK нет. Требование это раннера, а не наше — все метки по
умолчанию в документации Gitea указывают на node-образы. Поэтому образ несёт
ноду, а .NET целиком ставит `setup-dotnet`, ровно как на GitHub-стороне.

Плата за это — SDK обеих версий качается каждый прогон, и раннеру нужен
исходящий доступ к серверам Microsoft. Снимается собственным образом
(`sdk` + `nodejs` + рантайм 9), но его место — рядом с конфигурацией раннера,
а не в этом репозитории: образ общий для всех .NET-проектов.

## Токены

### GitHub

Встроенный `GITHUB_TOKEN`. Он не истекает, не требует ручного обновления и
ограничен этим репозиторием; права выдаёт блок `permissions` в воркфлоу.
Отдельный PAT не нужен.

Одного блока `permissions` при этом мало, и это неочевидно. `GITHUB_TOKEN`
публикует только пакеты, **связанные с репозиторием прогона**. Связь пакета с
репозиторием — отдельная сущность: GitHub Packages заводит её в момент создания
пакета по полю `repository` в nuspec, которое `dotnet pack` берёт из
`RepositoryUrl`. Отсюда требование: `RepositoryUrl` (задан один раз в
[Directory.Build.props](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/blob/master/Directory.Build.props))
обязан совпадать с фактическим адресом репозитория.

Если публикация отвечает `403 Forbidden`, связи нет и её заводят вручную:

> Package → Package settings → Manage Actions access → Add repository,
> роль **Write** для `andrey-aka-skif/AndreyAkaSkif.ServiceDefaults`.

Пакетов здесь пять — грант нужен каждому. Выдаётся он именно репозиторию, не
пользователю.

Связь заводится один раз, при создании пакета, и новыми выпусками не
переустанавливается: исправленный `RepositoryUrl` задним числом её не создаст.
Поэтому выданный грант снимать нельзя. Пересоздание пакета ради «чистоты» тоже
не выход — там ждёт
[известная проблема первой публикации](https://github.com/orgs/community/discussions/159893):
самый первый push из Actions отвечает 403, потому что выдать доступ до
появления пакета негде.

### Gitea

PAT в секрете `GT_PACKAGES_TOKEN`. Автоматический `GITEA_TOKEN` не имеет
авторизации на запись в реестр пакетов — документация Gitea прямо предписывает
обходиться персональным токеном.

Внутренний Gitea отдаёт HTTPS с самоподписанным сертификатом, а `dotnet`
смотрит в системное хранилище ОС и не читает ни `NODE_EXTRA_CA_CERTS`, ни
`GIT_SSL_CAINFO`, которыми раннер раздаёт свой CA. Решено на стороне раннера
(`container.options` в его `config.yaml`) — в репозитории для этого делать
ничего не нужно.
