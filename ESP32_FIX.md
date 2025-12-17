# 🔧 Исправление ESP32 для работы с сервером

## ❌ Проблема

ESP32 не может использовать `localhost` - это работает только на том же компьютере. ESP32 нужно использовать:
- **IP адрес вашего Mac** в локальной сети
- **Или URL через Cloudflare Tunnel** (если настроен)

## ✅ Решение

### Вариант 1: Использовать IP адрес Mac (для локальной сети)

1. **Узнайте IP адрес вашего Mac:**
   ```bash
   ifconfig | grep "inet " | grep -v 127.0.0.1
   ```
   
   Ваш IP: `192.168.2.170`

2. **Обновите код ESP32:**
   ```cpp
   const char* serverUrl = "http://192.168.2.170:8080/sensor/receive";
   ```

3. **Загрузите код в ESP32**

### Вариант 2: Использовать Cloudflare Tunnel URL (для интернета)

Если настроен Cloudflare Tunnel:

1. **Узнайте URL вашего туннеля:**
   - Зайдите в [Cloudflare Zero Trust](https://one.dash.cloudflare.com/)
   - Networks → Tunnels → ваш туннель → Public Hostnames
   - Скопируйте URL (например: `https://api.yourdomain.com`)

2. **Обновите код ESP32:**
   ```cpp
   const char* serverUrl = "https://api.yourdomain.com/sensor/receive";
   ```

3. **Загрузите код в ESP32**

## 📝 Текущий IP адрес вашего Mac

```
192.168.2.170
```

Используйте этот IP в коде ESP32, если тестируете в локальной сети.

## 🔄 Обновление кода

Я уже обновил файл `ESP32Storage/esp32.cpp` - замените `localhost` на `192.168.2.170`.

## ✅ Проверка

После обновления кода:

1. Загрузите код в ESP32
2. Откройте Serial Monitor
3. Должно появиться: `✓ Успешно отправлено! Код: 200`

## 🌐 Для фронтенда

Фронтенд уже настроен правильно - использует `REACT_APP_API_URL` из переменных окружения.

**Обновите в GitHub Secrets:**
- `REACT_APP_API_URL` = `http://192.168.2.170:8080` (для локальной сети)
- Или `REACT_APP_API_URL` = `https://api.yourdomain.com` (через Cloudflare Tunnel)

После обновления секрета, перезапустите деплой фронтенда.
