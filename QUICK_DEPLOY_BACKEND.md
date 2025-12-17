# 🚀 Быстрый деплой бэкенда

## ✅ Что уже готово

- ✅ Dockerfile создан
- ✅ Workflow для сборки образа настроен
- ✅ Cloudflare Tunnel токен есть
- ✅ Entrypoint script с поддержкой туннеля

## 📋 Что нужно сделать

### 1. Добавить секреты в GitHub (если еще не добавлены)

GitHub → **Settings** → **Secrets and variables** → **Actions**:

- ✅ `CLOUDFLARE_TUNNEL_TOKEN` = `eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9`
- ⚠️ `POSTGRES_CONNECTION_STRING` - ваша строка подключения к PostgreSQL
- ⚠️ `FRONTEND_URL` - URL фронтенда (например: `https://oxygen-sensor-frontend.pages.dev`)

### 2. Настроить Cloudflare Tunnel маршрутизацию

1. Зайдите в [Cloudflare Zero Trust](https://one.dash.cloudflare.com/)
2. **Networks** → **Tunnels** → найдите ваш туннель
3. Нажмите **Configure**
4. Добавьте **Public Hostname**:
   - **Subdomain**: `api`
   - **Domain**: ваш домен (или `stivkivi.workers.dev`)
   - **Service**: `http://localhost:8080`
5. Сохраните

### 3. Запустить сборку Docker образа

```bash
git add .
git commit -m "Add backend deployment workflow"
git push origin feature/DeploDocker
```

После push, GitHub Actions автоматически:
- ✅ Соберет Docker образ
- ✅ Загрузит его в GitHub Container Registry
- ✅ Покажет инструкции по деплою

### 4. Деплой на сервер

#### Вариант A: Простая команда

```bash
# 1. Войдите в GitHub Container Registry
echo "YOUR_GITHUB_TOKEN" | docker login ghcr.io -u Fek1r --password-stdin

# 2. Запустите контейнер
docker run -d \
  --name oxygen-sensor-backend \
  --restart unless-stopped \
  -p 8080:8080 \
  -e ConnectionStrings__PostgresConnection="Host=your-db;Port=5432;Database=sensordb;Username=user;Password=pass" \
  -e CLOUDFLARE_TUNNEL_TOKEN="eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9" \
  -e AllowedOrigins__0="https://oxygen-sensor-frontend.pages.dev" \
  ghcr.io/Fek1r/Oxygen-Sensor-Project/backend:latest
```

#### Вариант B: Docker Compose

```bash
# 1. Создайте .env файл
cat > .env << EOF
POSTGRES_CONNECTION_STRING=Host=your-db;Port=5432;Database=sensordb;Username=user;Password=pass
CLOUDFLARE_TUNNEL_TOKEN=eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9
FRONTEND_URL=https://oxygen-sensor-frontend.pages.dev
EOF

# 2. Войдите в GitHub Container Registry
echo "YOUR_GITHUB_TOKEN" | docker login ghcr.io -u Fek1r --password-stdin

# 3. Запустите
docker-compose -f docker-compose.prod.yml up -d
```

### 5. Проверка

```bash
# Проверьте логи
docker logs oxygen-sensor-backend

# Проверьте статус
docker ps | grep oxygen-sensor-backend

# Проверьте API локально
curl http://localhost:8080/sensor/all
```

### 6. Обновите фронтенд

После деплоя бэкенда, обновите `REACT_APP_API_URL` в GitHub Secrets на URL вашего API (например: `https://api.yourdomain.com`).

## 🔄 Обновление

При каждом push в `feature/DeploDocker`, новый образ собирается автоматически.

Для обновления на сервере:
```bash
docker pull ghcr.io/Fek1r/Oxygen-Sensor-Project/backend:latest
docker stop oxygen-sensor-backend
docker rm oxygen-sensor-backend
# Затем запустите снова (команда из шага 4)
```

## 📝 Где взять GitHub Token?

1. GitHub → **Settings** → **Developer settings** → **Personal access tokens** → **Tokens (classic)**
2. **Generate new token (classic)**
3. Выберите scope: `read:packages`
4. Скопируйте токен

## 🆘 Проблемы?

- **Контейнер не запускается**: `docker logs oxygen-sensor-backend`
- **Туннель не работает**: Проверьте токен и статус в Cloudflare Dashboard
- **БД не подключается**: Проверьте строку подключения

---

**Подробная инструкция**: см. `BACKEND_DEPLOY.md`
