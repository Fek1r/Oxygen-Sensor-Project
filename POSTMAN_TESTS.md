# 🧪 Тестирование API через Postman

## ✅ Статус

По логам видно, что:
- ✅ Cloudflare Tunnel подключен (`Registered tunnel connection`)
- ✅ Туннель работает (location: fra16, protocol: quic)
- ⚠️ Есть предупреждение про origin certificate (не критично при использовании токена)

## 🔍 Как проверить работу

### 1. Локальная проверка (на вашем Mac)

```bash
# Проверить что бэкенд работает локально
curl http://localhost:8080/sensor/all

# Проверить другие эндпоинты
curl http://localhost:8080/sensor/latest
curl http://localhost:8080/sensor/devices
```

### 2. Проверка через Cloudflare Tunnel

**Важно:** Нужно знать URL вашего туннеля. Есть два варианта:

#### Вариант A: Если настроена маршрутизация DNS

Если вы настроили маршрутизацию (например, `api.yourdomain.com`):

```bash
curl https://api.yourdomain.com/sensor/all
```

#### Вариант B: Если используете быстрый туннель

Если запустили `cloudflared tunnel --url http://localhost:8080`, то в логах будет показан временный URL вида:
```
https://xxxxx.trycloudflare.com
```

Используйте этот URL.

### 3. Как узнать URL вашего туннеля

```bash
# Проверить логи туннеля
cloudflared tunnel logs <tunnel-name>

# Или проверить в Cloudflare Dashboard
# Zero Trust → Networks → Tunnels → ваш туннель → Public Hostnames
```

## 📮 Тестирование в Postman

### Шаг 1: Откройте Postman

### Шаг 2: Создайте новый запрос

**Метод:** `GET`  
**URL:** 
- Локально: `http://localhost:8080/sensor/all`
- Через туннель: `https://api.yourdomain.com/sensor/all` (или ваш URL)

### Шаг 3: Доступные эндпоинты для тестирования

#### 1. Получить все данные
```
GET http://localhost:8080/sensor/all
```

#### 2. Получить последние данные
```
GET http://localhost:8080/sensor/latest
```

#### 3. Получить список устройств
```
GET http://localhost:8080/sensor/devices
```

#### 4. Получить историю данных
```
GET http://localhost:8080/sensor/history?period=hour
GET http://localhost:8080/sensor/history?period=day&mac=XX:XX:XX:XX:XX:XX
```

#### 5. Отправить данные от ESP32 (тест)
```
POST http://localhost:8080/sensor/receive
Content-Type: application/json

{
  "MAC": "AA:BB:CC:DD:EE:FF",
  "name": "ESP32 Test",
  "temp": 22.5,
  "hum": 45.0,
  "co2": 450
}
```

#### 6. Экспорт CSV
```
GET http://localhost:8080/sensor/export/csv?period=day
```

### Шаг 4: Настройка для тестирования через туннель

Если тестируете через Cloudflare Tunnel:

1. **URL:** Используйте ваш публичный URL (например: `https://api.yourdomain.com`)
2. **Headers:** Обычно не нужны, но если есть CORS проблемы, добавьте:
   ```
   Origin: https://oxygen-sensor-frontend.pages.dev
   ```

## 🔧 Проверка работы туннеля

### Проверить что туннель работает:

```bash
# В логах должно быть:
# "Registered tunnel connection" ✅
# "location=fra16" ✅
# "protocol=quic" ✅
```

### Проверить маршрутизацию:

1. Зайдите в [Cloudflare Zero Trust Dashboard](https://one.dash.cloudflare.com/)
2. **Networks** → **Tunnels**
3. Найдите ваш туннель
4. Проверьте **Public Hostnames** - должен быть настроен URL

## 🆘 Проблемы?

### Ошибка "Connection refused" в Postman
- Убедитесь, что бэкенд запущен: `docker ps | grep backend`
- Проверьте логи: `docker logs oxygen-sensor-backend`

### Ошибка "404 Not Found" через туннель
- Проверьте, что маршрутизация настроена в Cloudflare Dashboard
- Убедитесь, что используете правильный URL

### Ошибка CORS
- Проверьте, что `AllowedOrigins` включает ваш фронтенд URL
- В Postman добавьте Header: `Origin: https://oxygen-sensor-frontend.pages.dev`

## ✅ Быстрая проверка

Выполните в терминале:

```bash
# 1. Проверить локально
curl http://localhost:8080/sensor/all

# 2. Проверить через туннель (замените на ваш URL)
curl https://api.yourdomain.com/sensor/all

# 3. Проверить логи бэкенда
docker logs oxygen-sensor-backend | tail -20

# 4. Проверить статус контейнеров
docker ps
```

Если все команды работают - сервер работает! ✅
