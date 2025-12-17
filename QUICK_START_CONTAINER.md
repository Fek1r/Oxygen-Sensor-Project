# 🚀 Быстрый запуск контейнера

## Контейнер не запущен? Запустите его сейчас!

### Вариант 1: Использовать скрипт (проще всего)

```bash
# 1. Установите GitHub Token
export GITHUB_TOKEN=your_github_token_here

# 2. Запустите скрипт
./START_CONTAINER.sh
```

### Вариант 2: Ручной запуск

#### Шаг 1: Войдите в GitHub Container Registry

```bash
# Получите GitHub Personal Access Token:
# GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic
# Scope: read:packages

echo "YOUR_GITHUB_TOKEN" | docker login ghcr.io -u Fek1r --password-stdin
```

#### Шаг 2: Создайте .env файл

```bash
cat > .env << 'EOF'
POSTGRES_CONNECTION_STRING=Host=postgres;Port=5432;Database=postgres;Username=fek1r;Password=10021711
CLOUDFLARE_TUNNEL_TOKEN=eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9
FRONTEND_URL=https://oxygen-sensor-frontend.pages.dev
POSTGRES_PASSWORD=10021711
EOF
```

#### Шаг 3: Запустите контейнеры

```bash
docker-compose -f docker-compose.prod.yml up -d
```

### Вариант 3: Отдельный контейнер (если БД уже запущена)

Если PostgreSQL уже запущен на вашем Mac:

```bash
# 1. Войдите в GitHub Container Registry
echo "YOUR_GITHUB_TOKEN" | docker login ghcr.io -u Fek1r --password-stdin

# 2. Запустите только бэкенд
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

## ✅ Проверка

После запуска:

```bash
# Проверьте статус контейнеров
docker ps

# Проверьте логи бэкенда
docker logs oxygen-sensor-backend

# Проверьте API
curl http://localhost:8080/sensor/all
```

## 🆘 Проблемы?

### Ошибка "unauthorized" при docker login
- Проверьте правильность GitHub Token
- Убедитесь, что токен имеет scope `read:packages`

### Ошибка "No such image"
- Убедитесь, что вы вошли в GitHub Container Registry
- Попробуйте: `docker pull ghcr.io/Fek1r/Oxygen-Sensor-Project/backend:latest`

### Контейнер не запускается
- Проверьте логи: `docker logs oxygen-sensor-backend`
- Проверьте, что порт 8080 свободен: `lsof -i :8080`

### База данных не подключается
- Если используете docker-compose, убедитесь, что postgres запущен
- Проверьте строку подключения в .env файле
