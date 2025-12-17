# 🌐 Cloudflare Tunnel - Команды для терминала

## 🚀 Быстрый старт

### Вариант 1: Интерактивное меню (проще всего)

```bash
./setup-cloudflare-tunnel.sh
```

Откроется меню с выбором действий.

### Вариант 2: Прямые команды

```bash
./cloudflare-commands.sh <команда>
```

---

## 📋 Основные команды

### 1. Проверить версию
```bash
./cloudflare-commands.sh check
# или
cloudflared --version
```

### 2. Список туннелей
```bash
./cloudflare-commands.sh list
# или
cloudflared tunnel list
```

### 3. Создать новый туннель
```bash
./cloudflare-commands.sh create oxygen-backend
# или
cloudflared tunnel create oxygen-backend
```

### 4. Настроить маршрутизацию
```bash
./cloudflare-commands.sh route oxygen-backend api yourdomain.com
# или
cloudflared tunnel route dns oxygen-backend api.yourdomain.com
```

### 5. Запустить туннель с токеном
```bash
./cloudflare-commands.sh run
# или
export TUNNEL_TOKEN="eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9"
cloudflared tunnel run
```

### 6. Быстрый туннель (временный URL)
```bash
./cloudflare-commands.sh quick
# или
cloudflared tunnel --url http://localhost:8080
```

### 7. Информация о туннеле
```bash
./cloudflare-commands.sh info oxygen-backend
# или
cloudflared tunnel info oxygen-backend
```

### 8. Логи туннеля
```bash
./cloudflare-commands.sh logs oxygen-backend
# или
cloudflared tunnel logs oxygen-backend
```

### 9. Удалить туннель
```bash
./cloudflare-commands.sh delete oxygen-backend
# или
cloudflared tunnel delete oxygen-backend
```

---

## 🎯 Типичные сценарии

### Сценарий 1: Настроить туннель для бэкенда

```bash
# 1. Проверить список туннелей
./cloudflare-commands.sh list

# 2. Если туннеля нет - создать
./cloudflare-commands.sh create oxygen-backend

# 3. Настроить маршрутизацию
./cloudflare-commands.sh route oxygen-backend api yourdomain.com

# 4. Запустить туннель (в фоне или в отдельном терминале)
./cloudflare-commands.sh run
```

### Сценарий 2: Быстрый тест (временный URL)

```bash
# Запустить быстрый туннель
./cloudflare-commands.sh quick

# Будет показан временный URL вида: https://xxxxx.trycloudflare.com
# Используйте этот URL для тестирования
```

### Сценарий 3: Проверить работу туннеля

```bash
# 1. Проверить информацию
./cloudflare-commands.sh info oxygen-backend

# 2. Посмотреть логи
./cloudflare-commands.sh logs oxygen-backend
```

---

## ⚙️ Настройка config файла

Если используете именованный туннель, создайте файл `~/.cloudflared/config.yml`:

```yaml
tunnel: oxygen-backend
credentials-file: ~/.cloudflared/<tunnel-id>.json

ingress:
  - hostname: api.yourdomain.com
    service: http://localhost:8080
  - service: http_status:404
```

**Как найти tunnel-id:**
```bash
cloudflared tunnel list
```

---

## 🔧 Установка cloudflared

Если cloudflared не установлен:

### Mac:
```bash
brew install cloudflared
```

### Linux:
```bash
wget https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64.deb
sudo dpkg -i cloudflared-linux-amd64.deb
```

### Windows:
Скачайте с: https://developers.cloudflare.com/cloudflare-one/connections/connect-apps/install-and-setup/installation/

---

## 📝 Ваш токен туннеля

```
eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9
```

Этот токен уже используется в Docker контейнере, но вы можете использовать его и локально.

---

## ✅ Проверка работы

После запуска туннеля:

```bash
# Проверить что бэкенд работает локально
curl http://localhost:8080/sensor/all

# Проверить через туннель (замените на ваш URL)
curl https://api.yourdomain.com/sensor/all
```

---

## 🆘 Проблемы?

### Туннель не запускается
- Проверьте токен
- Убедитесь, что бэкенд работает на localhost:8080
- Проверьте логи: `./cloudflare-commands.sh logs <tunnel_name>`

### DNS не работает
- Убедитесь, что домен подключен к Cloudflare
- Проверьте настройки DNS в Cloudflare Dashboard

### Ошибка подключения
- Проверьте, что порт 8080 открыт
- Убедитесь, что бэкенд запущен: `docker ps | grep backend`
