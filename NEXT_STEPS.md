# ✅ Следующие шаги после успешной сборки

## 🎉 Что уже готово

- ✅ Docker образ собран и загружен в GitHub Container Registry
- ✅ Образ доступен по адресу: `ghcr.io/Fek1r/Oxygen-Sensor-Project/backend:latest`

## 📋 Шаг 1: Получить GitHub Personal Access Token

Для доступа к образам в GitHub Container Registry нужен токен:

1. Зайдите на GitHub → **Settings** → **Developer settings** → **Personal access tokens** → **Tokens (classic)**
2. Нажмите **Generate new token (classic)**
3. Название: `Docker Registry Access`
4. Выберите scope: **`read:packages`**
5. Нажмите **Generate token**
6. **Скопируйте токен** (показывается только один раз!)

## 📋 Шаг 2: Настроить Cloudflare Tunnel маршрутизацию

1. Зайдите в [Cloudflare Zero Trust Dashboard](https://one.dash.cloudflare.com/)
2. Перейдите: **Networks** → **Tunnels**
3. Найдите ваш туннель (или создайте новый)
4. Нажмите **Configure** рядом с туннелем
5. Добавьте **Public Hostname**:
   - **Subdomain**: `api` (или любое другое)
   - **Domain**: выберите ваш домен (или используйте `stivkivi.workers.dev`)
   - **Service**: `http://localhost:8080`
6. Сохраните

После этого ваш API будет доступен по адресу: `https://api.yourdomain.com`

## 📋 Шаг 3: Деплой на сервер

### Вариант A: Docker Compose (рекомендуется)

```bash
# 1. Создайте .env файл на сервере
cat > .env << 'EOF'
POSTGRES_CONNECTION_STRING=Host=postgres;Port=5432;Database=postgres;Username=fek1r;Password=10021711
CLOUDFLARE_TUNNEL_TOKEN=eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9
FRONTEND_URL=https://oxygen-sensor-frontend.pages.dev
POSTGRES_PASSWORD=10021711
EOF

# 2. Войдите в GitHub Container Registry
echo "YOUR_GITHUB_TOKEN" | docker login ghcr.io -u Fek1r --password-stdin

# 3. Запустите контейнеры
docker-compose -f docker-compose.prod.yml up -d
```

### Вариант B: Отдельный контейнер (если БД уже запущена)

```bash
# 1. Войдите в GitHub Container Registry
echo "YOUR_GITHUB_TOKEN" | docker login ghcr.io -u Fek1r --password-stdin

# 2. Запустите контейнер
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

**Замените:**
- `YOUR_GITHUB_TOKEN` - токен из Шага 1
- `host.docker.internal` - на `172.17.0.1` или IP хоста, если Linux сервер

## 📋 Шаг 4: Проверка работы

```bash
# Проверьте логи
docker logs oxygen-sensor-backend

# Проверьте статус контейнера
docker ps | grep oxygen-sensor-backend

# Проверьте API локально
curl http://localhost:8080/sensor/all

# Проверьте через Cloudflare Tunnel
curl https://api.yourdomain.com/sensor/all
```

## 📋 Шаг 5: Обновить фронтенд

После успешного деплоя бэкенда, обновите секрет `REACT_APP_API_URL` в GitHub:

1. GitHub → **Settings** → **Secrets and variables** → **Actions**
2. Найдите `REACT_APP_API_URL`
3. Обновите на URL вашего API (например: `https://api.yourdomain.com`)
4. Перезапустите деплой фронтенда (или просто сделайте push)

## 🔄 Обновление бэкенда в будущем

При каждом push в `feature/DeploDocker`, новый образ собирается автоматически.

Для обновления на сервере:

```bash
# Остановите старый контейнер
docker stop oxygen-sensor-backend
docker rm oxygen-sensor-backend

# Загрузите новый образ
docker pull ghcr.io/Fek1r/Oxygen-Sensor-Project/backend:latest

# Запустите снова (используйте команду из Шага 3)
```

## 🆘 Решение проблем

### Контейнер не запускается
```bash
docker logs oxygen-sensor-backend
```
Проверьте:
- Правильность строки подключения к БД
- Доступность базы данных
- Правильность токена Cloudflare Tunnel

### Cloudflare Tunnel не работает
- Проверьте статус туннеля в Cloudflare Dashboard
- Убедитесь, что маршрутизация настроена правильно
- Проверьте логи: `docker logs oxygen-sensor-backend`

### База данных не подключается
- Проверьте, что БД запущена и доступна
- Для Docker: используйте правильный Host (postgres, host.docker.internal, или IP)
- Проверьте логи приложения

## ✅ Готово!

После выполнения всех шагов:
- ✅ Бэкенд работает в Docker контейнере
- ✅ Cloudflare Tunnel обеспечивает безопасный доступ
- ✅ Фронтенд может подключаться к API
- ✅ ESP32 может отправлять данные на API

---

**Нужна помощь?** Проверьте:
- `DEPLOY_NOW.md` - готовые команды
- `BACKEND_DEPLOY.md` - подробная инструкция
- `POSTGRES_CONNECTION_STRING.md` - правильный формат строки подключения
