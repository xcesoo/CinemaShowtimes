# Cinema Showtimes API

REST API для управління кінотеатром, сеансами та бронюванням квитків. Проєкт побудований за принципами Clean Architecture, Domain-Driven Design (DDD) та CQRS, з фокусом на правильне моделювання бізнес-процесів та надійність даних.

## Технологічний стек
* Платформа: .NET 9 / C# 13
* Архітектура: Clean Architecture, CQRS (MediatR)
* База даних: PostgreSQL + Entity Framework Core 9
* Тестування: xUnit, Moq, FluentAssertions, WebApplicationFactory
* Інфраструктура: Docker, Docker Compose

---

## Реалізовані вимоги (User Stories)

У проєкті виконані всі базові та додаткові (Optional) вимоги тестового завдання:

* US-1 та US-2: Створення фільмів та сеансів (API + автоматичний Seeding при старті).
* US-3: Бронювання місць із перевіркою на перетин з іншими активними бронюваннями та проданими місцями.
* US-4: Підтвердження (викуп) бронювання з обмеженням у 10 хвилин.
* US-5: Збереження даних у реляційній базі PostgreSQL.
* US-6 (Optional): Алгоритм пошуку та бронювання суміжних місць для компаній.
* US-7 (Optional): Захист від подвійного бронювання під час одночасних запитів. Для операцій бронювання використовується рівень ізоляції PostgreSQL `Serializable`. У разі конфлікту серіалізації одна з транзакцій завершується з SQLSTATE `40001`, після чого API повертає `409 Conflict`.

---

## Архітектура та Структура проєкту

Проєкт розділений на 4 незалежні шари для забезпечення слабкої зв'язності (Clean Architecture). Нижче наведено повну структуру файлів та їхній зв'язок із бізнес-вимогами:
```
CinemaShowtimes/
├── docker-compose.yaml                      
├── .dockerignore                            
├── CinemaShowtimes.sln                      
│
├── CinemaShowtimes.Api/                     # Точка входу
│   ├── Controllers/
│   │   ├── MoviesController.cs              # Ендпоінти управління фільмами [US-1]
│   │   ├── ReservationsController.cs        # Ендпоінти бронювання та викупу [US-3, US-4, US-6, US-7]
│   │   └── ShowtimesController.cs           # Ендпоінти створення та перегляду сеансів [US-2]
│   ├── Middlewares/
│   │   └── GlobalExceptionsHandler.cs       # Перехоплює помилки БД і віддає 409 Conflict [US-7]
│   ├── CinemaShowtimes.Api.csproj           # Файл проєкту
│   ├── Dockerfile                           # Інструкція збірки контейнера
│   └── Program.cs                           # Ініціалізація, DI контейнер, налаштування Swagger
│
├── CinemaShowtimes.Application/             # Бізнес-логіка (CQRS Handlers)
│   ├── Commands/
│   │   ├── Movies/
│   │   │   ├── CreateMovieCommand.cs
│   │   │   └── CreateMovieCommandHandler.cs             # Створення фільму [US-1]
│   │   ├── Reservations/
│   │   │   ├── ConfirmReservationCommand.cs
│   │   │   ├── ConfirmReservationCommandHandler.cs      # Підтвердження броні, перевірка часу [US-4]
│   │   │   ├── ReserveContiguousSeatsCommand.cs
│   │   │   ├── ReserveContiguousSeatsCommandHandler.cs  # Алгоритм пошуку суміжних місць [US-6]
│   │   │   ├── ReserveSeatsCommand.cs
│   │   │   └── ReserveSeatsCommandHandler.cs            # Стандартне бронювання, перевірка доступності [US-3]
│   │   └── Showtimes/
│   │       ├── CreateShowtimeCommand.cs
│   │       └── CreateShowtimeCommandHandler.cs          # Створення сеансу [US-2]
│   ├── DTOs/
│   │   ├── ReservationResultDto.cs          # DTO успішного бронювання
│   │   ├── ReserveSeatRequest.cs            # DTO запиту місця
│   │   └── SeatMapDto.cs                    # DTO карти вільних/зайнятих місць
│   ├── Queries/                             # CQRS запити на читання (Auditoriums, Movies, Reservations, Showtimes)
│   │   ├── Auditoriums/ ...
│   │   ├── Movies/ ...
│   │   ├── Reservations/ ...
│   │   └── Showtimes/ ...
│   ├── CinemaShowtimes.Application.csproj
│   └── DependencyInjection.cs               # Реєстрація MediatR
│
├── CinemaShowtimes.Domain/                  # Домен системи
│   ├── Entities/
│   │   ├── Auditorium.cs                    # Сутність залу
│   │   ├── Movie.cs                         # Сутність фільму [US-1]
│   │   ├── Reservation.cs                   # Сутність броні, логіка протухання через 10 хвилин [US-3, US-4]
│   │   └── Showtime.cs                      # Сутність сеансу [US-2]
│   ├── Exceptions/
│   │   └── DomainException.cs               # Кастомний ексепшн для бізнес-правил
│   ├── Interfaces/
│   │   ├── IAuditoriumRepository.cs
│   │   ├── IDomainTransaction.cs            # Абстракція транзакцій домену
│   │   ├── IMovieRepository.cs
│   │   ├── IReservationRepository.cs
│   │   ├── IShowtimeRepository.cs
│   │   └── IUnitOfWork.cs                   # Патерн Unit of Work [US-5, US-7]
│   ├── ValueObjects/
│   │   └── Seat.cs                          # Інкапсулює валідацію місця (ряд та номер > 0)
│   └── CinemaShowtimes.Domain.csproj
│
├── CinemaShowtimes.Infrastructure/          # Робота з БД та зовнішніми сервісами
│   ├── Extensions/
│   │   └── ApplicationBuilderExtensions.cs  # Автоматичне застосування міграцій, та сідинг бази
│   ├── Migrations/                          # Згенеровані міграції EF Core
│   │   ├── 20260818134932_AddModelsConfiguration.cs
│   │   ├── 20260818134932_AddModelsConfiguration.Designer.cs
│   │   └── CinemaDbContextModelSnapshot.cs
│   ├── Persistence/
│   │   ├── Configurations/                  # Fluent API конфігурації таблиць
│   │   │   ├── AuditoriumConfiguration.cs
│   │   │   ├── MovieConfiguration.cs
│   │   │   ├── ReservationConfiguration.cs
│   │   │   └── ShowtimeConfiguration.cs
│   │   ├── CinemaDbContext.cs               # Контекст EF Core та реалізація Unit of Work [US-5, US-7]
│   │   ├── DatabaseSeeder.cs                # Автоматично створює тестовий зал та фільми при старті
│   │   └── EfTransactionWrapper.cs          # Адаптер EF Core транзакції до доменного інтерфейсу [US-7]
│   ├── Repositories/                        # Реалізації інтерфейсів домену
│   │   ├── AuditoriumRepository.cs
│   │   ├── MovieRepository.cs
│   │   ├── ReservationRepository.cs
│   │   └── ShowtimeRepository.cs
│   ├── CinemaShowtimes.Infrastructure.csproj
│   └── DependencyInjection.cs               # Реєстрація DbContext та репозиторіїв
│
├── CinemaShowtimes.Tests/                   # Юніт та інтеграційні тести
│   ├── Application/                         # Тести CQRS хендлерів та алгоритмів
│   ├── Domain/                              # Тести бізнес-правил та таймінгів (FakeTimeProvider)
│   ├── Helpers/                             # Допоміжні класи (моки транзакцій)
│   ├── Integration/                         
│   │   ├── MoviesControllerTests.cs         # Smoke-тест API
│   │   └── ReservationsControllerTests.cs   # End-to-End тести на конкурентність та подвійне бронювання [US-7]
│   └── CinemaShowtimes.Tests.csproj
└─
```

## Інструкція із запуску (Docker)

Проєкт повністю контейнеризований. Не потрібно встановлювати .NET SDK чи PostgreSQL локально. При старті додаток автоматично створить базу даних, застосує міграції та заповнить її початковими даними (фільми та сеанси).

1. Переконайтеся, що встановлений та запущений Docker.
2. Відкрийте термінал у корені проєкту та виконайте команду:

```bash
docker compose up -d --build
```

3. Після запуску відкрийте браузер та перейдіть за адресою:
   http://localhost:8080/swagger

Щоб зупинити проєкт:

```bash
docker compose down
```

---

## Тестування

Проєкт покритий автоматизованими тестами (15 тестів). Для керування часом під час тестування термінів дії бронювання використовується `FakeTimeProvider`.

Види тестів у `CinemaShowtimes.Tests`:

* **Domain Tests:** Перевірка бізнес-правил доменних сутностей, зокрема терміну дії бронювання та валідації місць.
* **Application Tests:** Перевірка логіки команд і запитів, включаючи бронювання зайнятих місць, пошук суміжних місць та формування карти місць.
* **Integration Tests:** Перевірка API через реальні HTTP-запити до тестового ASP.NET Core сервера, включаючи сценарій конкурентного бронювання.

Результат запуску тестів:

```text
Test summary: total: 15; failed: 0; succeeded: 15; skipped: 0
```

Запуск тестів (локально, якщо встановлено .NET 9 SDK):

```bash
dotnet test
```

---