# Правильный формат POSTGRES_CONNECTION_STRING

## ✅ Ваша строка подключения (формат правильный)

```
Host=localhost;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```

**Формат полностью правильный для .NET/Npgsql!** ✅

## ⚠️ Но есть нюанс с Docker

### Проблема с `localhost` в Docker

Если база данных находится **вне Docker контейнера** (на хосте), то `localhost` внутри контейнера будет указывать на сам контейнер, а не на хост.

### Решения для разных сценариев:

## 📋 Сценарий 1: БД на хосте (вне Docker)

### Для Mac/Windows (Docker Desktop):
```
Host=host.docker.internal;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```

### Для Linux:
```
Host=172.17.0.1;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```
*(172.17.0.1 - это обычно IP адрес docker0 интерфейса)*

Или используйте реальный IP адрес хоста:
```bash
# Узнайте IP хоста
ip addr show docker0 | grep inet
# Используйте этот IP вместо localhost
```

## 📋 Сценарий 2: БД в Docker Compose (в той же сети)

Если используете `docker-compose.yml`, используйте **имя сервиса**:

```
Host=postgres;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```

*(`postgres` - это имя сервиса в docker-compose.yml)*

## 📋 Сценарий 3: БД в отдельном Docker контейнере

Если PostgreSQL запущен в отдельном контейнере, используйте:

### Вариант A: Docker network
```
Host=oxygen-sensor-postgres;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```
*(`oxygen-sensor-postgres` - имя контейнера с БД)*

### Вариант B: --link (устаревший способ)
```
Host=postgres;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```

## 📋 Сценарий 4: БД на удаленном сервере

Если PostgreSQL на удаленном сервере (например, Supabase, Railway, Neon):

```
Host=your-db-host.com;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```

## ✅ Рекомендации

### Для разработки (локально):
```
Host=localhost;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```
✅ Работает, если запускаете приложение напрямую (не в Docker)

### Для Docker Compose:
```
Host=postgres;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```
✅ Работает, если БД в том же docker-compose

### Для Docker контейнера + БД на хосте:
```
Host=host.docker.internal;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```
✅ Работает на Mac/Windows

```
Host=172.17.0.1;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```
✅ Работает на Linux

## 🔍 Как проверить правильность

1. **Локально (без Docker)**: `Host=localhost` ✅
2. **Docker Compose**: `Host=postgres` ✅
3. **Docker + БД на хосте (Mac/Windows)**: `Host=host.docker.internal` ✅
4. **Docker + БД на хосте (Linux)**: `Host=172.17.0.1` или IP хоста ✅
5. **Удаленная БД**: `Host=your-db-host.com` ✅

## 📝 Примеры для GitHub Secrets

### Если используете Docker Compose:
```
POSTGRES_CONNECTION_STRING=Host=postgres;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```

### Если БД на сервере (вне Docker):
```
POSTGRES_CONNECTION_STRING=Host=host.docker.internal;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```

### Если БД на удаленном сервере:
```
POSTGRES_CONNECTION_STRING=Host=your-db-host.com;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```

## 🎯 Итог

Ваш формат **абсолютно правильный**! Просто замените `localhost` на:
- `postgres` - если БД в docker-compose
- `host.docker.internal` - если БД на хосте (Mac/Windows)
- IP адрес - если БД на хосте (Linux)
- Имя контейнера - если БД в отдельном контейнере
- Хост удаленного сервера - если БД удаленная
