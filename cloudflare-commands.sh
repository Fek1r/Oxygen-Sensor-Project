#!/bin/bash

# Быстрые команды Cloudflare Tunnel для Oxygen Sensor Project

# Ваш токен туннеля
TUNNEL_TOKEN="eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9"

# Цвета
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo -e "${GREEN}═══════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}  Cloudflare Tunnel - Быстрые команды${NC}"
echo -e "${GREEN}═══════════════════════════════════════════════════════════${NC}"
echo ""

case "$1" in
    check)
        echo -e "${YELLOW}Проверка версии:${NC}"
        cloudflared --version
        ;;
    
    list)
        echo -e "${YELLOW}Список туннелей:${NC}"
        cloudflared tunnel list
        ;;
    
    create)
        if [ -z "$2" ]; then
            echo "Использование: $0 create <tunnel_name>"
            exit 1
        fi
        echo -e "${YELLOW}Создание туннеля: $2${NC}"
        cloudflared tunnel create "$2"
        ;;
    
    route)
        if [ -z "$2" ] || [ -z "$3" ] || [ -z "$4" ]; then
            echo "Использование: $0 route <tunnel_name> <subdomain> <domain>"
            echo "Пример: $0 route my-tunnel api yourdomain.com"
            exit 1
        fi
        echo -e "${YELLOW}Настройка маршрутизации: $2 -> $3.$4${NC}"
        cloudflared tunnel route dns "$2" "${3}.${4}"
        ;;
    
    run)
        echo -e "${YELLOW}Запуск туннеля с токеном...${NC}"
        export TUNNEL_TOKEN="$TUNNEL_TOKEN"
        cloudflared tunnel run
        ;;
    
    info)
        if [ -z "$2" ]; then
            echo "Использование: $0 info <tunnel_name>"
            exit 1
        fi
        echo -e "${YELLOW}Информация о туннеле: $2${NC}"
        cloudflared tunnel info "$2"
        ;;
    
    logs)
        if [ -z "$2" ]; then
            echo "Использование: $0 logs <tunnel_name>"
            exit 1
        fi
        echo -e "${YELLOW}Логи туннеля: $2${NC}"
        cloudflared tunnel logs "$2" | tail -50
        ;;
    
    delete)
        if [ -z "$2" ]; then
            echo "Использование: $0 delete <tunnel_name>"
            exit 1
        fi
        echo -e "${YELLOW}Удаление туннеля: $2${NC}"
        read -p "Вы уверены? (yes/no): " confirm
        if [ "$confirm" = "yes" ]; then
            cloudflared tunnel delete "$2"
        else
            echo "Отменено"
        fi
        ;;
    
    quick)
        echo -e "${YELLOW}Быстрый туннель на localhost:8080${NC}"
        echo "Туннель будет доступен по временному URL"
        cloudflared tunnel --url http://localhost:8080
        ;;
    
    *)
        echo "Использование: $0 <команда> [параметры]"
        echo ""
        echo "Команды:"
        echo "  check                    - Проверить версию cloudflared"
        echo "  list                     - Список всех туннелей"
        echo "  create <name>            - Создать новый туннель"
        echo "  route <name> <sub> <dom> - Настроить DNS маршрутизацию"
        echo "  run                      - Запустить туннель с токеном"
        echo "  info <name>              - Информация о туннеле"
        echo "  logs <name>              - Показать логи туннеля"
        echo "  delete <name>            - Удалить туннель"
        echo "  quick                    - Быстрый туннель (временный URL)"
        echo ""
        echo "Примеры:"
        echo "  $0 check"
        echo "  $0 list"
        echo "  $0 create oxygen-backend"
        echo "  $0 route oxygen-backend api yourdomain.com"
        echo "  $0 run"
        echo "  $0 info oxygen-backend"
        echo "  $0 logs oxygen-backend"
        exit 1
        ;;
esac
