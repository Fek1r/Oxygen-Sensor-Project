# ✅ Чеклист проверки настроек

## Ваши данные Cloudflare

- **Account ID**: `aa2f7fc5de004e56e4c0189edbdd6ed7` ✅
- **Subdomain**: `stivkivi.workers.dev`
- **API Token**: `De3FKowMsH1z86SQsba4c609CrtvQBorlkW0_aa1` ✅
- **Tunnel Token**: `eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9` ✅

## Проверка GitHub Secrets

Убедитесь, что в GitHub репозитории добавлены секреты:

1. ✅ **CLOUDFLARE_API_TOKEN** = `De3FKowMsH1z86SQsba4c609CrtvQBorlkW0_aa1`
2. ✅ **CLOUDFLARE_ACCOUNT_ID** = `aa2f7fc5de004e56e4c0189edbdd6ed7`
3. ✅ **REACT_APP_API_URL** = `http://localhost:5245` (или ваш API URL)
4. ✅ **CLOUDFLARE_TUNNEL_TOKEN** = `eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9`

## Проверка Cloudflare Pages проекта

### ⚠️ ВАЖНО: Проект должен быть создан вручную!

1. Зайдите на https://dash.cloudflare.com/
2. Перейдите: **Workers & Pages** → **Pages**
3. Проверьте, что проект `oxygen-sensor-frontend` существует
4. Если проекта нет - создайте его:
   - Нажмите **Create a project**
   - Название: `oxygen-sensor-frontend`
   - Выберите **Upload assets**
   - Нажмите **Create project**

## Проверка workflow

Workflow настроен правильно:
- ✅ Использует правильный Account ID из секретов
- ✅ Использует правильный API Token из секретов
- ✅ Название проекта: `oxygen-sensor-frontend`
- ✅ Собирает из `frontend/build`
- ✅ Отключен строгий режим сборки (`CI: false`)

## Что делать дальше

1. ✅ Убедитесь, что проект `oxygen-sensor-frontend` создан в Cloudflare Pages
2. ✅ Проверьте, что все секреты добавлены в GitHub
3. Запушьте изменения:
   ```bash
   git add .
   git commit -m "Deploy frontend"
   git push origin feature/DeploDocker
   ```
4. Проверьте вкладку **Actions** в GitHub - workflow должен запуститься
5. После успешного деплоя, ваш фронтенд будет доступен по адресу:
   ```
   https://oxygen-sensor-frontend.pages.dev
   ```

## Возможные проблемы

### Ошибка 404 "Could not route to /client/v4/accounts/***/pages/projects"
- **Причина**: Проект не создан в Cloudflare Pages
- **Решение**: Создайте проект вручную (см. выше)

### Ошибка "API Token is invalid"
- **Причина**: Неправильный токен или недостаточно прав
- **Решение**: Проверьте токен в GitHub Secrets

### Ошибка сборки
- **Причина**: Проблемы с кодом или зависимостями
- **Решение**: Проверьте логи в GitHub Actions
