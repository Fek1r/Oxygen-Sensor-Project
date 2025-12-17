# 🚀 Готовые команды для деплоя бэкенда

## Ваша строка подключения к БД

```
Host=localhost;Port=5432;Database=postgres;Username=fek1r;Password=10021711
```

## ⚠️ Важно: localhost в Docker

Если база данных находится на том же сервере, но вне Docker, замените `localhost` на:
- `host.docker.internal` (для Docker Desktop на Mac/Windows)
- IP адрес хоста (для Linux)
- Или используйте docker-compose (см. ниже)

## 📋 Вариант 1: Docker Compose (рекомендуется)

Используйте готовый `docker-compose.prod.yml` с вашей БД:

```bash
# 1. Создайте .env файл
cat > .env << 'EOF'
POSTGRES_CONNECTION_STRING=Host=postgres;Port=5432;Database=postgres;Username=fek1r;Password=10021711
CLOUDFLARE_TUNNEL_TOKEN=eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9
FRONTEND_URL=https://oxygen-sensor-frontend.pages.dev
POSTGRES_PASSWORD=10021711
EOF

# 2. Войдите в GitHub Container Registry
echo "YOUR_GITHUB_TOKEN" | docker login ghcr.io -u Fek1r --password-stdin

# 3. Запустите (с локальной БД в compose)
docker-compose -f docker-compose.prod.yml up -d postgres
docker-compose -f docker-compose.prod.yml up -d backend
```

## 📋 Вариант 2: Отдельный контейнер (БД на хосте)

Если PostgreSQL уже запущен на сервере (не в Docker):

```bash
# 1. Войдите в GitHub Container Registry
echo "YOUR_GITHUB_TOKEN" | docker login ghcr.io -u Fek1r --password-stdin

# 2. Запустите контейнер с host.docker.internal (Mac/Windows) или IP хоста (Linux)
# Для Linux замените host.docker.internal на IP адрес сервера (например: 172.17.0.1)

docker run -d \
  --name oxygen-sensor-backend \
  --restart unless-stopped \
  --add-host=host.docker.internal:host-gateway \
  -p 8080:8080 \
  -e ConnectionStrings__PostgresConnection="Host=host.docker.internal;Port=5432;Database=postgres;Username=fek1r;Password=10021711" \
  -e CLOUDFLARE_TUNNEL_TOKEN="eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9" \
  -e AllowedOrigins__0="https://oxygen-sensor-frontend.pages.dev" \
  ghcr.io/Fek1r/Oxygen-Sensor-Project/backend:latest
```

**Для Linux сервера:**
```bash
# Узнайте IP адрес хоста
ip addr show docker0 | grep inet

# Используйте этот IP вместо host.docker.internal
# Например: Host=172.17.0.1
```

## 📋 Вариант 3: БД в отдельном контейнере

Если хотите запустить PostgreSQL в отдельном контейнере:

```bash
# 1. Запустите PostgreSQL
docker run -d \
  --name oxygen-sensor-postgres \
  --restart unless-stopped \
  -e POSTGRES_DB=postgres \
  -e POSTGRES_USER=fek1r \
  -e POSTGRES_PASSWORD=10021711 \
  -p 5432:5432 \
  postgres:16-alpine

# 2. Подождите 5 секунд пока БД запустится
sleep 5

# 3. Войдите в GitHub Container Registry
echo "YOUR_GITHUB_TOKEN" | docker login ghcr.io -u Fek1r --password-stdin

# 4. Запустите бэкенд (используйте имя контейнера postgres)
docker run -d \
  --name oxygen-sensor-backend \
  --restart unless-stopped \
  --link oxygen-sensor-postgres:postgres \
  -p 8080:8080 \
  -e ConnectionStrings__PostgresConnection="Host=postgres;Port=5432;Database=postgres;Username=fek1r;Password=10021711" \
  -e CLOUDFLARE_TUNNEL_TOKEN="eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9" \
  -e AllowedOrigins__0="https://oxygen-sensor-frontend.pages.dev" \
  ghcr.io/Fek1r/Oxygen-Sensor-Project/backend:latest
```

## ✅ Проверка

```bash
# Проверьте логи бэкенда
docker logs oxygen-sensor-backend

# Проверьте статус
docker ps

# Проверьте API
curl http://localhost:8080/sensor/all
```

## 🔄 Обновление

```bash
# Остановите старый контейнер
docker stop oxygen-sensor-backend
docker rm oxygen-sensor-backend

# Загрузите новый образ
docker pull ghcr.io/Fek1r/Oxygen-Sensor-Project/backend:latest

# Запустите снова (используйте команду из варианта выше)
```

## 📝 GitHub Secrets

Убедитесь, что в GitHub добавлены секреты:

- `POSTGRES_CONNECTION_STRING` = `Host=postgres;Port=5432;Database=postgres;Username=fek1r;Password=10021711`
- `CLOUDFLARE_TUNNEL_TOKEN` = `eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9`
- `FRONTEND_URL` = `https://oxygen-sensor-frontend.pages.dev`
