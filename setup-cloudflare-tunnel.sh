#!/bin/bash

# Цвета для вывода
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${GREEN}═══════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}  Настройка Cloudflare Tunnel для Oxygen Sensor Project${NC}"
echo -e "${GREEN}═══════════════════════════════════════════════════════════${NC}"
echo ""

# Проверка установки cloudflared
echo -e "${YELLOW}📦 Проверка cloudflared...${NC}"
if ! command -v cloudflared &> /dev/null; then
    echo -e "${RED}❌ cloudflared не установлен${NC}"
    echo ""
    echo "Установите cloudflared:"
    echo "  Mac: brew install cloudflared"
    echo "  Linux: wget https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64.deb && sudo dpkg -i cloudflared-linux-amd64.deb"
    echo "  Или: https://developers.cloudflare.com/cloudflare-one/connections/connect-apps/install-and-setup/installation/"
    exit 1
fi

VERSION=$(cloudflared --version | head -n1)
echo -e "${GREEN}✅ cloudflared установлен: ${VERSION}${NC}"
echo ""

# Меню выбора действия
echo "Выберите действие:"
echo "1) Проверить версию cloudflared"
echo "2) Список туннелей"
echo "3) Создать новый туннель"
echo "4) Настроить маршрутизацию для существующего туннеля"
echo "5) Запустить туннель (с токеном)"
echo "6) Проверить статус туннеля"
echo "7) Показать логи туннеля"
echo "8) Удалить туннель"
echo "9) Выход"
echo ""
read -p "Введите номер (1-9): " choice

case $choice in
    1)
        echo ""
        echo -e "${GREEN}Версия cloudflared:${NC}"
        cloudflared --version
        ;;
    
    2)
        echo ""
        echo -e "${GREEN}Список туннелей:${NC}"
        cloudflared tunnel list
        ;;
    
    3)
        echo ""
        read -p "Введите имя туннеля: " tunnel_name
        echo ""
        echo -e "${YELLOW}Создание туннеля: ${tunnel_name}${NC}"
        cloudflared tunnel create "$tunnel_name"
        echo ""
        echo -e "${GREEN}✅ Туннель создан!${NC}"
        echo ""
        echo "Следующие шаги:"
        echo "1. Настройте маршрутизацию (опция 4)"
        echo "2. Или используйте токен для быстрого туннеля (опция 5)"
        ;;
    
    4)
        echo ""
        echo -e "${YELLOW}Настройка маршрутизации${NC}"
        echo ""
        read -p "Введите имя туннеля: " tunnel_name
        read -p "Введите subdomain (например: api): " subdomain
        read -p "Введите домен (например: yourdomain.com или stivkivi.workers.dev): " domain
        read -p "Введите локальный порт (по умолчанию 8080): " port
        port=${port:-8080}
        
        echo ""
        echo -e "${YELLOW}Настройка DNS маршрутизации...${NC}"
        cloudflared tunnel route dns "$tunnel_name" "${subdomain}.${domain}"
        
        echo ""
        echo -e "${GREEN}✅ Маршрутизация настроена!${NC}"
        echo ""
        echo "Теперь настройте config файл:"
        echo "  ~/.cloudflared/config.yml"
        echo ""
        echo "Пример конфигурации:"
        echo "---"
        echo "tunnel: $tunnel_name"
        echo "credentials-file: ~/.cloudflared/$(cloudflared tunnel list | grep "$tunnel_name" | awk '{print $1}').json"
        echo ""
        echo "ingress:"
        echo "  - hostname: ${subdomain}.${domain}"
        echo "    service: http://localhost:${port}"
        echo "  - service: http_status:404"
        echo "---"
        ;;
    
    5)
        echo ""
        echo -e "${YELLOW}Запуск туннеля с токеном${NC}"
        echo ""
        echo "Ваш токен туннеля:"
        echo "eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9"
        echo ""
        read -p "Запустить туннель? (y/n): " confirm
        if [ "$confirm" = "y" ]; then
            echo ""
            echo -e "${GREEN}Запуск туннеля...${NC}"
            echo "Туннель будет работать в фоне"
            echo "Для остановки нажмите Ctrl+C"
            echo ""
            export TUNNEL_TOKEN="eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9"
            cloudflared tunnel run
        fi
        ;;
    
    6)
        echo ""
        read -p "Введите имя туннеля: " tunnel_name
        echo ""
        echo -e "${GREEN}Информация о туннеле:${NC}"
        cloudflared tunnel info "$tunnel_name"
        ;;
    
    7)
        echo ""
        read -p "Введите имя туннеля: " tunnel_name
        echo ""
        echo -e "${GREEN}Логи туннеля (последние 50 строк):${NC}"
        cloudflared tunnel logs "$tunnel_name" | tail -50
        ;;
    
    8)
        echo ""
        read -p "Введите имя туннеля для удаления: " tunnel_name
        echo ""
        echo -e "${RED}⚠️  ВНИМАНИЕ: Это действие нельзя отменить!${NC}"
        read -p "Вы уверены? (yes/no): " confirm
        if [ "$confirm" = "yes" ]; then
            cloudflared tunnel delete "$tunnel_name"
            echo -e "${GREEN}✅ Туннель удален${NC}"
        else
            echo "Отменено"
        fi
        ;;
    
    9)
        echo "Выход..."
        exit 0
        ;;
    
    *)
        echo -e "${RED}Неверный выбор${NC}"
        exit 1
        ;;
esac

echo ""
echo -e "${GREEN}═══════════════════════════════════════════════════════════${NC}"
