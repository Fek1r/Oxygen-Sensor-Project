# Деплой бэкенда

## 🎯 Что нужно для деплоя бэкенда

1. ✅ Docker образ собирается автоматически при push в GitHub
2. ⚠️ Нужен сервер с Docker для запуска контейнера
3. ⚠️ Нужна база данных PostgreSQL
4. ✅ Cloudflare Tunnel токен уже есть

## 📋 Шаг 1: Проверка секретов в GitHub

Убедитесь, что добавлены все секреты:

- ✅ `CLOUDFLARE_TUNNEL_TOKEN` = `eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9`
- ⚠️ `POSTGRES_CONNECTION_STRING` - строка подключения к PostgreSQL
- ⚠️ `FRONTEND_URL` - URL вашего фронтенда (например: `https://oxygen-sensor-frontend.pages.dev`)

## 📋 Шаг 2: Настройка Cloudflare Tunnel

### 2.1. Проверьте туннель в Cloudflare

1. Зайдите в [Cloudflare Zero Trust Dashboard](https://one.dash.cloudflare.com/)
2. Перейдите: **Networks** → **Tunnels**
3. Найдите туннель (или создайте новый, если нужно)
4. Нажмите **Configure** рядом с туннелем

### 2.2. Настройте маршрутизацию

Добавьте **Public Hostname**:
- **Subdomain**: `api` (или любое другое)
- **Domain**: выберите ваш домен (или используйте `stivkivi.workers.dev`)
- **Service**: `http://localhost:8080`
- Сохраните

После этого ваш API будет доступен по адресу: `https://api.yourdomain.com` (или `https://api.stivkivi.workers.dev`)

## 📋 Шаг 3: Деплой на сервер

### Вариант A: Деплой на ваш сервер (VPS, домашний сервер)

#### 3.1. Подготовка сервера

```bash
# Установите Docker (если еще не установлен)
curl -fsSL https://get.docker.com -o get-docker.sh
sh get-docker.sh

# Установите Docker Compose (опционально)
sudo apt-get install docker-compose-plugin
```

#### 3.2. Создайте Personal Access Token в GitHub

1. GitHub → **Settings** → **Developer settings** → **Personal access tokens** → **Tokens (classic)**
2. Нажмите **Generate new token (classic)**
3. Выберите scope: `read:packages`
4. Скопируйте токен

#### 3.3. Запустите контейнер

```bash
# Войдите в GitHub Container Registry
echo "YOUR_GITHUB_TOKEN" | docker login ghcr.io -u YOUR_GITHUB_USERNAME --password-stdin

# Запустите контейнер
docker run -d \
  --name oxygen-sensor-backend \
  --restart unless-stopped \
  -p 8080:8080 \
  -e ConnectionStrings__PostgresConnection="Host=your-db-host;Port=5432;Database=sensordb;Username=user;Password=pass" \
  -e CLOUDFLARE_TUNNEL_TOKEN="eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9" \
  -e AllowedOrigins__0="https://oxygen-sensor-frontend.pages.dev" \
  ghcr.io/Fek1r/Oxygen-Sensor-Project/backend:latest
```

**Замените:**
- `YOUR_GITHUB_TOKEN` - ваш GitHub Personal Access Token
- `YOUR_GITHUB_USERNAME` - ваш GitHub username (Fek1r)
- `ConnectionStrings__PostgresConnection` - строка подключения к вашей БД

### Вариант B: Использовать docker-compose

Создайте файл `docker-compose.prod.yml`:

```yaml
version: '3.8'

services:
  backend:
    image: ghcr.io/Fek1r/Oxygen-Sensor-Project/backend:latest
    container_name: oxygen-sensor-backend
    restart: unless-stopped
    ports:
      - "8080:8080"
    environment:
      - ConnectionStrings__PostgresConnection=${POSTGRES_CONNECTION_STRING}
      - CLOUDFLARE_TUNNEL_TOKEN=${CLOUDFLARE_TUNNEL_TOKEN}
      - AllowedOrigins__0=${FRONTEND_URL}
    # Для доступа к GitHub Container Registry
    # Сначала выполните: echo "GITHUB_TOKEN" | docker login ghcr.io -u Fek1r --password-stdin

  # Опционально: PostgreSQL в том же compose
  postgres:
    image: postgres:16-alpine
    container_name: oxygen-sensor-postgres
    environment:
      - POSTGRES_DB=sensordb
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

Запуск:
```bash
# Создайте .env файл
cat > .env << EOF
POSTGRES_CONNECTION_STRING=Host=postgres;Port=5432;Database=sensordb;Username=postgres;Password=postgres
CLOUDFLARE_TUNNEL_TOKEN=eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9
FRONTEND_URL=https://oxygen-sensor-frontend.pages.dev
EOF

# Войдите в GitHub Container Registry
echo "YOUR_GITHUB_TOKEN" | docker login ghcr.io -u Fek1r --password-stdin

# Запустите
docker-compose -f docker-compose.prod.yml up -d
```

## 📋 Шаг 4: Проверка работы

### 4.1. Проверьте контейнер

```bash
# Посмотрите логи
docker logs oxygen-sensor-backend

# Проверьте статус
docker ps | grep oxygen-sensor-backend
```

### 4.2. Проверьте Cloudflare Tunnel

1. Зайдите в Cloudflare Zero Trust Dashboard
2. Проверьте статус туннеля - должен быть **Active**
3. Проверьте, что маршрутизация настроена правильно

### 4.3. Проверьте API

```bash
# Локально на сервере
curl http://localhost:8080/sensor/all

# Через Cloudflare Tunnel
curl https://api.yourdomain.com/sensor/all
```

## 📋 Шаг 5: Обновите фронтенд

После деплоя бэкенда, обновите секрет `REACT_APP_API_URL` в GitHub:

1. GitHub → **Settings** → **Secrets and variables** → **Actions**
2. Найдите `REACT_APP_API_URL`
3. Обновите на URL вашего API (например: `https://api.yourdomain.com`)
4. Перезапустите деплой фронтенда

## 🔄 Обновление бэкенда

При каждом push в ветку `feature/DeploDocker`, новый Docker образ будет собран автоматически.

Для обновления на сервере:

```bash
# Остановите старый контейнер
docker stop oxygen-sensor-backend
docker rm oxygen-sensor-backend

# Загрузите новый образ
docker pull ghcr.io/Fek1r/Oxygen-Sensor-Project/backend:latest

# Запустите новый контейнер (используйте те же команды, что и выше)
```

## 🆘 Решение проблем

### Контейнер не запускается
- Проверьте логи: `docker logs oxygen-sensor-backend`
- Проверьте переменные окружения
- Убедитесь, что порт 8080 свободен

### Cloudflare Tunnel не работает
- Проверьте токен туннеля
- Проверьте статус туннеля в Cloudflare Dashboard
- Проверьте логи контейнера

### База данных не подключается
- Проверьте строку подключения
- Убедитесь, что БД доступна с сервера
- Проверьте логи приложения

## ✅ Готово!

После выполнения всех шагов:
- ✅ Бэкенд будет работать в Docker контейнере
- ✅ Cloudflare Tunnel обеспечит безопасный доступ
- ✅ Фронтенд сможет подключаться к API
