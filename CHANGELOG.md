# Журнал изменений

Формат основан на [Keep a Changelog](https://keepachangelog.com/ru/1.1.0/).
Этот проект придерживается [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

Журнал ведётся с версии 0.0.46. Заметки к более ранним выпускам —
в [Releases](https://github.com/andrey-aka-skif/AndreyAkaSkif.ServiceDefaults/releases).

## [0.0.46] - 2026-08-13

Технический выпуск. Публичный API и целевые фреймворки не менялись; из
изменений, доходящих до потребителя пакетов, — только патч одной зависимости.

### Изменено

- `Microsoft.Extensions.Hosting.Abstractions` обновлён с 10.0.10 до 10.0.11 —
  зависимость пакета `AndreyAkaSkif.ServiceDefaults.PostgreSQL`, остальных
  пакетов обновление не касается
- воркфлоу приведены к общему стандарту CI/CD: публикация переименована в
  `publish.yml` и переведена на встроенный `GITHUB_TOKEN`, разбор релизного тега
  вынесен в общий композитный экшен `.ci/actions/prepare-nuget-release`
- в CI добавлены проверка упаковки (`dotnet pack`) и сборка документации

### Добавлено

- зеркальный набор воркфлоу для внутреннего инстанса Gitea (`.gitea/workflows`)
- `CONTRIBUTING.md`: устройство репозитория, порядок выпуска, токены обоих
  реестров
