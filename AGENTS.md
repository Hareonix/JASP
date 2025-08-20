# Мастер-промт для ChatGPT

Ты — старший разработчик. Твоя задача: с нуля спроектировать и написать **рабочий** проект «Панель управления sing-box VPN» со стеком:

* **Backend:** C# / .NET 8 (Minimal APIs), EF Core, SQLite, FluentValidation, AutoMapper
* **Frontend:** Vue 3 (Composition API) + **Nuxt 3**, **Vuetify 3**, Monaco Editor, Pinia, vue-router (через Nuxt), предпочтительно `chart.js` (через `vue-chartjs`) для графиков
* **Коммуникации:** REST + WebSocket (SignalR Core или собственный WS)
* **Контейнеризация:** Docker / docker-compose
* **Тесты:** xUnit (backend), Vitest (frontend)
* **Линтинг/формат:** .editorconfig, ESLint + Prettier, dotnet analyzers

## Цель

Сделать менеджер конфигураций **sing-box** с максимальной кастомизацией:

* Хранить и редактировать **Inbound**/**Outbound** как произвольный JSON по схеме sing-box.
* Назначать **клиентов к Outbound** (моделируем конечных пользователей/устройства).
* Подписки (**Subscription groups**): выдавать агрегированную конфигурацию как endpoint (JSON) с токеном, кэшированием и ETag.
* Интерфейс редактирования Inbound «через код» — встроить **Monaco Editor** + валидация по JSON Schema.
* Отчёты/дашборд: список клиентов, онлайн-клиенты, список Inbound-ов, нагрузка на CPU, базовые метрики.
* Live-статус по WebSocket: онлайн-клиенты, обновление метрик без перезагрузки.

> ВАЖНО: придерживайся **актуальной схемы sing-box**. Для полей, которые меняются между версиями, выдели слой адаптации и укажи, где править JSON Schema. Не «хардкоди» значения — валидацию делай против схемы.

## Что именно нужно от тебя

1. **Архитектура и структура репозитория** (monorepo):

   ```
   / (root)
   ├─ backend/ (C# .NET 8 Minimal APIs)
   ├─ frontend/ (Nuxt 3 + Vuetify 3)
   ├─ deploy/ (docker-compose, env-шаблоны, миграции)
   └─ docs/ (README, диаграммы, OpenAPI)
   ```

2. **Схема БД (EF Core + миграции)**:

   * `Users` (Id, Email, PasswordHash, Role\[Admin|Operator|Viewer], CreatedAt)
   * `Clients` (Id, Name, Tags\[], IsOnline, LastSeenAt, Notes)
   * `Inbounds` (Id, Name, Type, Version, JsonConfig (JSONB), Enabled, CreatedAt, UpdatedAt)
   * `Outbounds` (Id, Name, Type, Version, JsonConfig (JSONB), Enabled, CreatedAt, UpdatedAt)
   * `ClientOutbounds` (ClientId, OutboundId, Priority, Enabled)
   * `SubscriptionGroups` (Id, Name, Slug, Notes)
   * `SubscriptionGroupMembers` (GroupId, OutboundId, Order)
   * `SubscriptionTokens` (Id, GroupId, Token, ExpiresAt, RateLimitPerMin, IsRevoked)
   * `MetricsSamples` (Id, Timestamp, CpuLoadPct, TotalClients, OnlineClients, Extra JSONB)

3. **API (OpenAPI/Swagger) — основные эндпоинты**:

   * Auth: `POST /api/auth/login` (JWT), `GET /api/auth/me`
   * Inbounds: `GET/POST/PUT/DELETE /api/inbounds`, `POST /api/inbounds/{id}/validate`, `GET /api/inbounds/{id}/render`
   * Outbounds: `GET/POST/PUT/DELETE /api/outbounds`, `POST /api/outbounds/{id}/validate`, `GET /api/outbounds/{id}/render`
   * Clients: `GET/POST/PUT/DELETE /api/clients`, `GET /api/clients/online`
   * Привязка клиентов к outbounds: `PUT /api/clients/{id}/outbounds` (массив связей с приоритетом)
   * Subscription groups:
     `GET/POST/PUT/DELETE /api/subscriptions/groups`,
     `GET/POST/DELETE /api/subscriptions/groups/{id}/members`,
     `GET/POST/DELETE /api/subscriptions/groups/{id}/tokens`
   * Выдача подписки (публично с токеном):
     `GET /sub/{token}/singbox.json` — агрегированный JSON с учётом порядка/флагов; заголовки `ETag`, `Cache-Control`, `Content-Type: application/json`
   * Метрики/дашборд:
     `GET /api/stats/overview` (всего клиентов/онлайн/активных inbound/outbound),
     `GET /api/stats/cpu` (серия точек),
     `GET /api/stats/inbounds` (список),
     `GET /api/stats/clients` (срез)
   * WebSocket/SignalR: `/ws` — каналы `metrics`, `clientsOnline` (сердцебиение, reconnect)

4. **Синхронизация с sing-box (абстракция провайдера)**:

   * Интерфейс `ISingBoxProvider`: `Validate(json)`, `Apply(configBundle)`, `Reload()`, `GetRuntimeStats()`
   * Реализации:

     * **LocalProcess**: пишет `config.json` в путь, перезапускает сервис (systemd service или Windows service).
     * **Docker**: монтирует конфиг в контейнер, делает `docker exec` для reload, опционально healthcheck.
   * `configBundle` формируется из выбранных Inbounds/Outbounds. Поддержать «сухой прогон» (dry-run).

5. **Валидация схемы sing-box**:

   * Хранить актуальную **JSON Schema** в `backend/resources/schemas/sing-box/*.json`.
   * Валидация на бэкенде (FluentValidation + JSON Schema validator).
   * На фронте — интегрировать **Monaco JSON language service** с этой схемой (подсветка, автодополнение).

6. **Frontend (Nuxt + Vuetify + Monaco)**:

   * Страницы:

     * `/login`
     * `/dashboard` — карточки: Всего клиентов, Онлайн, Inbounds, CPU; график CPU и онлайн-клиентов
     * `/clients` — таблица Vuetify (CRUD, поиск/фильтры/теги, массовые операции); диалог привязки Outbounds с приоритетами (drag\&drop)
     * `/inbounds` — таблица + страница редактирования: **Monaco Editor** (JSON), кнопки «Validate», «Render snippet»
     * `/outbounds` — таблица + страница редактирования: **Monaco Editor** (JSON), предпросмотр/валидация
     * `/subscriptions` — список групп, состав групп (упорядочиваемые Outbounds), выпуск/отзыв токенов (копируемая ссылка), превью JSON
     * `/settings` — провайдер sing-box (Local/Docker), путь к конфигу/контейнеру, параметры перезапуска, политика кэша подписок
   * Состояние: Pinia-store для auth, entities, live-метрик (WS)
   * Компоненты:

     * `JsonEditorMonaco.vue` (обвязка Monaco + JSON Schema)
     * `CrudTable.vue` (универсальная Vuetify таблица с колоночными фильтрами)
     * `MetricsChart.vue` (vue-chartjs)
     * `OutboundMappingDialog.vue` (привязка Outbounds к клиенту)

7. **Безопасность**

   * JWT, роли: Admin/Operator/Viewer (RBAC на эндпоинтах)
   * Подписочные токены — отдельная сущность, **только на чтение**, с лимитом RPS (встроенный rate-limit middleware)
   * CORS, HTTPS behind reverse proxy, защита от JSON-инъекций
   * ETag/If-None-Match для `/sub/{token}/singbox.json`

8. **Метрики и «онлайн-клиенты»**

   * CPU: HostedService, собирает каждые N секунд (кроссплатформенно: /proc/stat или `PerformanceCounter`/Diagnostics); сохраняет в `MetricsSamples`, ретеншн.
   * Онлайн-клиенты: модельно анализируем «последнюю активность» (например, через внешние логи/метрики, заглушка и интерфейс для подключения реального источника). Поставь интерфейс `IClientsPresenceSource` + InMemory mock.

9. **Пайплайн разработки**

   * Сначала: БД + миграции + базовые CRUD + валидация JSON Schema.
   * Далее: Subscription endpoints (с ETag/cache) + WS метрики.
   * Потом: интеграция провайдера sing-box (LocalProcess, затем Docker).
   * В конце: UI-полировка, роли, тесты, контейнеризация, README.

10. **Выходные артефакты (обязательно предоставить код):**

    * Полный **backend** проект (Program.cs, endpoints, слои: Domain/Application/Infrastructure или простая модульная разбивка; DTO/Entities/Validators/Mappers; миграции; интеграционные тесты важного).
    * Полный **frontend** (Nuxt config, Vuetify plugin, страницы/компоненты/сторы; интеграция Monaco с JSON Schema; графики).
    * **OpenAPI** yml/json, **docker-compose.yml** (Postgres + backend + frontend + sing-box контейнер-заглушка для demo).
    * Скрипт **seed** (создать админа, несколько inbound/outbound/clients, demo-группу и подписочный токен).
    * **README** с инструкциями запуска (`docker compose up -d`) и dev-режимом (hot reload).

11. **Качество кода**

    * Чистая архитектура, DI, разделение доменной логики и инфраструктуры.
    * Продуманные DTO (в т.ч. «typed helpers» для часто используемых полей sing-box).
    * Обработка ошибок (ProblemDetails), логирование (Serilog), трассировка запросов.
    * Покрыть тестами: валидацию схем, сборку `configBundle`, ETag-кэш.

12. **Демонстрация**

    * Покажи пошагово:

      1. структура репозитория;
      2. ключевые файлы backend (с кодом);
      3. ключевые файлы frontend (с кодом);
      4. docker-compose;
      5. seed;
      6. скриншоты/гифки (если можешь) или описания UI.

13. **Особые требования по редактированию JSON (Monaco)**

    * Включить JSON-схемы для `inbounds` и `outbounds` (подсветка ошибок, автодополнение).
    * Кнопки: «Validate» (локальная проверка) и «Validate (server)» для бэкенд-валидации.
    * Кнопка «Format» (причесать JSON).
    * Guard при выходе со страницы с несохранёнными изменениями.

14. **Про производительность и DX**

    * Пагинация/поиск/фильтры на CRUD-таблицах.
    * Индексы в БД по ключевым полям (Name, Enabled, UpdatedAt).
    * Кэширование подписок на 30–120 сек (настраиваемо).
    * CI-ready: команды для линтинга, тестов, сборки.

## Важно: как отвечать

* Сначала дай **короткий обзор** архитектуры.
* Далее **реальную структуру файлов** и **полные исходники** ключевых модулей (без «псевдокода»).
* Разбей ответ на **несколько сообщений**, если нужно, но каждое — с работающими фрагментами.
* Где уместно — приводи **минимальные, но запускаемые** примеры (особенно Program.cs, Nuxt pages, docker-compose).
* На местах, где требуются конкретные поля sing-box, используй **валидируемый JSON** и комментируй, где править схему при обновлениях.
* Обязательно предоставь **OpenAPI** схему и пример **/sub/{token}/singbox.json** ответа.

---

### Доп. параметры для кастомизации (можешь зашить по умолчанию, но вынести в env/config):

* `SUBSCRIPTION_CACHE_SECONDS=60`
* `METRICS_SAMPLING_SECONDS=5`
* `JWT_EXPIRES_MIN=60`
* `RATE_LIMIT_SUB_PER_MIN=60`
* Путь к локальному конфигу sing-box или имя Docker-контейнера `SINGBOX_CONTAINER=singbox`

---

Сгенерируй весь проект в соответствии с ТЗ выше. Если чего-то не хватает — **делай разумные дефолты**, оставляя комментарии, где менять.

---

**Ожидаемый результат:** после твоего ответа я смогу `docker compose up -d` и открыть UI, войти админом, создать/отредактировать inbound/outbound через Monaco, собрать подписку и увидеть JSON по публичной ссылке с токеном, а на дашборде — базовые графики и онлайн-состояние (мок) клиентов.
