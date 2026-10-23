# Лабораторная работа №4

**Тема:** Оптимизация проекта и разработка модульных тестов.

Тишин М. С., группа ВБИо-401рсоб. Преподаватель: Скопин В. И.
Дисциплина: Коллективная разработка приложений.

Работа развивает существующий C#-проект из [лабораторной №2](https://github.com/Crewar-wq/lab02/tree/main/src/Part1.Interfaces). Исходные файлы импортированы отдельным коммитом и сохранены в ветке `baseline-import`. Формулы, сообщения и успешный сценарий сохранены. Из монолитного файла выделены платежи, логирование и демонстрационные операции; файловая система доступна через `ILogStorage`. На границе приложения добавлена обработка `IOException` с кодом выхода 1.

## Структура

| Каталог | Содержимое |
|---|---|
| `src/Lab04.Core` | Исходные модели, платежи, устройства, логирование и файловый адаптер |
| `src/Lab04.App` | Консольный сценарий и обработка ошибок ввода-вывода |
| `tests/Lab04.Tests` | xUnit, Moq, модульные и две проверки файловой системы |
| `tools/Lab04.Performance` | Замеры времени и выделений памяти |
| `evidence` | Результаты проверок и снимки отчётов |
| `reports` | Отчёт MS Word |

## Запуск

Нужен .NET 8 SDK. Все NuGet-зависимости фиксируются в `packages.lock.json`.

```bash
dotnet restore Lab04.sln --locked-mode
dotnet build Lab04.sln -c Release --no-restore
dotnet run --project src/Lab04.App -c Release -- artifacts/work.log
dotnet test tests/Lab04.Tests -c Release --no-build --settings coverage.runsettings --collect:"XPlat Code Coverage" --results-directory artifacts/coverage
python3 scripts/check_coverage.py
```

Область покрытия — **весь исполняемый код двух рабочих сборок**, `Lab04.Core` и `Lab04.App`, включая точку входа и файловый адаптер. Измерительные программы, тесты, сторонние библиотеки и сгенерированные SDK-файлы к тестируемой бизнес-логике не относятся. Исключений исходных файлов или атрибутов `ExcludeFromCodeCoverage` нет.

Начальное покрытие получают тем же инструментом на той же версии кода, запуская один исходный тест:

```bash
dotnet test tests/Lab04.Tests -c Release --settings coverage.runsettings --collect:"XPlat Code Coverage" --results-directory artifacts/before --filter 'Stage=Baseline'
dotnet tool restore
dotnet tool run reportgenerator '-reports:artifacts/coverage/*/coverage.cobertura.xml' '-targetdir:artifacts/coverage-html' '-reporttypes:Html'
```

Для нового замера удалите предыдущий каталог `artifacts/coverage`, иначе несколько XML-отчётов не пройдут проверку целостности. Сборщик проверяет все строки и ветви; неполный или пустой отчёт приводит к ошибке CI.

## Автоматизация и производительность

GitHub Actions запускает сборку, начальный тест, полный набор, проверку покрытия, генерацию HTML и измерение производительности при каждом `push` и `pull_request`. XML, TRX, HTML и замеры доступны в артефакте `tests-coverage-performance`; время и память также выводятся в Job summary.

```bash
dotnet run --project tools/Lab04.Performance -c Release -- artifacts/performance.json
python3 scripts/performance_summary.py artifacts/performance.json
```

В `.vscode/tasks.json` находятся задачи сборки, тестов и замеров для запуска из IDE. Для подробного профиля откройте решение в Visual Studio, выберите Release, Debug → Performance Profiler → CPU Usage / .NET Object Allocation Tracking и измерительный проект `Lab04.Performance`.

Микрозамеры выполняются после прогрева, по 1 000 000 операций, девять повторов; берётся медиана. В результат входит накладное время цикла и вызова делегата. Значения разных машин не являются доказательством ускорения. 100% покрытия подтверждает выполнение строк и переходов, но не гарантирует полноту требований, всех входных данных или отсутствие дефектов.
