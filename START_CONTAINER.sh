#!/bin/bash

echo "🚀 Запуск бэкенда Oxygen Sensor Project"
echo ""

# Проверка наличия GitHub Token
if [ -z "$GITHUB_TOKEN" ]; then
    echo "⚠️  GITHUB_TOKEN не установлен"
    echo "Установите токен: export GITHUB_TOKEN=your_token"
    echo "Или введите токен сейчас:"
    read -s GITHUB_TOKEN
    export GITHUB_TOKEN
fi

# Вход в GitHub Container Registry
echo "📦 Вход в GitHub Container Registry..."
echo "$GITHUB_TOKEN" | docker login ghcr.io -u Fek1r --password-stdin

if [ $? -ne 0 ]; then
    echo "❌ Ошибка входа в GitHub Container Registry"
    exit 1
fi

echo "✅ Успешный вход"
echo ""

# Создание .env файла
echo "📝 Создание .env файла..."
cat > .env << 'EOF'
POSTGRES_CONNECTION_STRING=Host=postgres;Port=5432;Database=postgres;Username=fek1r;Password=10021711
CLOUDFLARE_TUNNEL_TOKEN=eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9
FRONTEND_URL=https://oxygen-sensor-frontend.pages.dev
POSTGRES_PASSWORD=10021711
EOF

echo "✅ .env файл создан"
echo ""

# Запуск контейнеров
echo "🐳 Запуск контейнеров..."
docker-compose -f docker-compose.prod.yml up -d

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Контейнеры запущены успешно!"
    echo ""
    echo "📊 Проверка статуса:"
    docker ps | grep oxygen-sensor
    echo ""
    echo "📋 Логи бэкенда:"
    echo "   docker logs oxygen-sensor-backend"
    echo ""
    echo "🌐 API доступен по адресу:"
    echo "   http://localhost:8080"
    echo ""
    echo "🔍 Проверка API:"
    echo "   curl http://localhost:8080/sensor/all"
else
    echo "❌ Ошибка запуска контейнеров"
    exit 1
fi
