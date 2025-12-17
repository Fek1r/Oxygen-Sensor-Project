# Пошаговая инструкция по настройке CI/CD

## Шаг 1: Настройка Cloudflare

### 1.1. Получите Cloudflare API Token

1. Зайдите в [Cloudflare Dashboard](https://dash.cloudflare.com/)
2. Перейдите в **My Profile** → **API Tokens**
3. Нажмите **Create Token**
4. Используйте шаблон **Edit Cloudflare Workers** или создайте кастомный токен с правами:
   - **Account** → **Cloudflare Pages** → **Edit**
   - **Account** → **Cloudflare Tunnel** → **Edit**
5. Скопируйте токен (он показывается только один раз!)

### 1.2. Найдите Account ID

1. В Cloudflare Dashboard выберите ваш домен
2. В правой панели найдите **Account ID** (скопируйте его)

### 1.3. Создайте Cloudflare Tunnel

1. Зайдите в [Cloudflare Zero Trust Dashboard](https://one.dash.cloudflare.com/)
2. Перейдите в **Networks** → **Tunnels**
3. Нажмите **Create a tunnel**
4. Выберите **Cloudflared** как тип коннектора
5. Дайте имя туннелю (например: `oxygen-sensor-backend`)
6. Выберите **Docker** как окружение
7. **Скопируйте токен туннеля** (Tunnel Token) - он понадобится позже

### 1.4. Настройте маршрутизацию туннеля

1. После создания туннеля, нажмите **Configure** рядом с туннелем
2. Добавьте **Public Hostname**:
   - **Subdomain**: `api` (или любое другое)
   - **Domain**: выберите ваш домен
   - **Service**: `http://localhost:8080`
3. Сохраните конфигурацию

## Шаг 2: Настройка GitHub Secrets

1. Зайдите в ваш GitHub репозиторий
2. Перейдите в **Settings** → **Secrets and variables** → **Actions**
3. Нажмите **New repository secret** и добавьте следующие секреты:

### Обязательные секреты:

| Имя секрета | Значение | Где взять |
|------------|----------|-----------|
| `CLOUDFLARE_API_TOKEN` | Ваш API токен Cloudflare | Шаг 1.1 |
| `CLOUDFLARE_ACCOUNT_ID` | Ваш Account ID | Шаг 1.2 |
| `CLOUDFLARE_TUNNEL_TOKEN` | Токен туннеля | Шаг 1.3 |
| `POSTGRES_CONNECTION_STRING` | Строка подключения к PostgreSQL | Ваша БД |
| `REACT_APP_API_URL` | URL вашего API | Например: `https://api.yourdomain.com` |
| `FRONTEND_URL` | URL фронтенда (опционально) | URL Cloudflare Pages |

### Пример строки подключения PostgreSQL:
```
Host=your-host.com;Port=5432;Database=sensordb;Username=user;Password=password
```

## Шаг 3: Настройка базы данных PostgreSQL

Убедитесь, что у вас есть доступная база данных PostgreSQL. Вы можете использовать:
- **Supabase** (бесплатный план)
- **Railway** (бесплатный план)
- **Neon** (бесплатный план)
- Или свой собственный сервер PostgreSQL

Скопируйте строку подключения и добавьте в GitHub Secrets как `POSTGRES_CONNECTION_STRING`.

## Шаг 4: Запуск CI/CD

1. Закоммитьте и запушьте изменения в ветку `main` или `dev`:
   ```bash
   git add .
   git commit -m "Add CI/CD pipeline"
   git push origin main
   ```

2. GitHub Actions автоматически запустится:
   - Соберет фронтенд и задеплоит на Cloudflare Pages
   - Соберет Docker образ бэкенда и загрузит в GitHub Container Registry

3. Проверьте статус в **Actions** вкладке вашего репозитория

## Шаг 5: Деплой бэкенда

После успешной сборки Docker образа, вам нужно задеплоить его на сервер:

### Вариант A: На вашем сервере

```bash
# Войдите в GitHub Container Registry
echo "YOUR_GITHUB_TOKEN" | docker login ghcr.io -u YOUR_USERNAME --password-stdin

# Запустите контейнер
docker run -d \
  --name oxygen-sensor-backend \
  --restart unless-stopped \
  -p 8080:8080 \
  -e ConnectionStrings__PostgresConnection="YOUR_CONNECTION_STRING" \
  -e CLOUDFLARE_TUNNEL_TOKEN="YOUR_TUNNEL_TOKEN" \
  -e AllowedOrigins__0="https://your-frontend.pages.dev" \
  ghcr.io/YOUR_USERNAME/Oxygen-Sensor-Project/backend:latest
```

### Вариант B: Используя docker-compose

Создайте файл `docker-compose.prod.yml`:

```yaml
version: '3.8'
services:
  backend:
    image: ghcr.io/YOUR_USERNAME/Oxygen-Sensor-Project/backend:latest
    container_name: oxygen-sensor-backend
    restart: unless-stopped
    ports:
      - "8080:8080"
    environment:
      - ConnectionStrings__PostgresConnection=${POSTGRES_CONNECTION_STRING}
      - CLOUDFLARE_TUNNEL_TOKEN=${CLOUDFLARE_TUNNEL_TOKEN}
      - AllowedOrigins__0=${FRONTEND_URL}
```

Запустите:
```bash
docker-compose -f docker-compose.prod.yml up -d
```

## Шаг 6: Проверка работы

1. **Фронтенд**: Откройте URL вашего Cloudflare Pages проекта
2. **Бэкенд**: Проверьте логи контейнера:
   ```bash
   docker logs oxygen-sensor-backend
   ```
3. **Туннель**: Проверьте статус в Cloudflare Zero Trust Dashboard

## Возможные проблемы

### Фронтенд не подключается к бэкенду
- Проверьте `REACT_APP_API_URL` в GitHub Secrets
- Убедитесь, что CORS настроен правильно (добавьте URL фронтенда в `AllowedOrigins`)

### Туннель не работает
- Проверьте `CLOUDFLARE_TUNNEL_TOKEN` в GitHub Secrets
- Убедитесь, что туннель настроен в Cloudflare Dashboard
- Проверьте логи: `docker logs oxygen-sensor-backend`

### Ошибки базы данных
- Проверьте строку подключения PostgreSQL
- Убедитесь, что база данных доступна из вашего сервера
- Проверьте логи приложения

## Полезные команды

```bash
# Посмотреть логи контейнера
docker logs oxygen-sensor-backend

# Перезапустить контейнер
docker restart oxygen-sensor-backend

# Обновить образ
docker pull ghcr.io/YOUR_USERNAME/Oxygen-Sensor-Project/backend:latest
docker stop oxygen-sensor-backend
docker rm oxygen-sensor-backend
# Затем запустите снова (см. Шаг 5)
```

## Готово! 🎉

После выполнения всех шагов:
- ✅ Фронтенд автоматически деплоится при каждом push
- ✅ Бэкенд собирается в Docker образ
- ✅ Cloudflare Tunnel обеспечивает безопасный доступ к бэкенду
