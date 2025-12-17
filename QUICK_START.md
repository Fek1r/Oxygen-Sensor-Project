# Быстрый старт - Деплой фронтенда

## ✅ Ваши данные Cloudflare

- **API Token**: `De3FKowMsH1z86SQsba4c609CrtvQBorlkW0_aa1`
- **Account ID**: `aa2f7fc5de004e56e4c0189edbdd6ed7`
- **Tunnel Token**: `eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9`

## 🚀 Шаги для деплоя фронтенда

### 1. Добавьте секреты в GitHub (2 минуты)

1. Откройте ваш репозиторий на GitHub
2. Перейдите: **Settings** → **Secrets and variables** → **Actions**
3. Нажмите **New repository secret** и добавьте:

#### Секрет 1: CLOUDFLARE_API_TOKEN
- **Name**: `CLOUDFLARE_API_TOKEN`
- **Value**: `De3FKowMsH1z86SQsba4c609CrtvQBorlkW0_aa1`

#### Секрет 2: CLOUDFLARE_ACCOUNT_ID
- **Name**: `CLOUDFLARE_ACCOUNT_ID`
- **Value**: `aa2f7fc5de004e56e4c0189edbdd6ed7`

#### Секрет 3: REACT_APP_API_URL
- **Name**: `REACT_APP_API_URL`
- **Value**: `http://localhost:5245` (пока используем локальный, потом обновим)

### 2. Закоммитьте и запушьте изменения

```bash
git add .
git commit -m "Add frontend deployment to Cloudflare Pages"
git push origin main
```

### 3. Проверьте деплой

1. Откройте вкладку **Actions** в вашем GitHub репозитории
2. Дождитесь завершения workflow "Deploy Frontend to Cloudflare Pages"
3. После успешного деплоя, ваш фронтенд будет доступен по адресу:
   ```
   https://oxygen-sensor-frontend.pages.dev
   ```

## 📝 Что происходит автоматически?

При каждом push в ветку `main` или `dev`:
- ✅ Устанавливаются зависимости npm
- ✅ Собирается React приложение
- ✅ Деплоится на Cloudflare Pages

## 🔧 Если что-то пошло не так

### Ошибка "API Token is invalid"
- Проверьте, что токен скопирован полностью
- Убедитесь, что токен имеет права на Cloudflare Pages

### Ошибка "Account ID not found"
- Проверьте Account ID в Cloudflare Dashboard
- Убедитесь, что используете правильный аккаунт

### Фронтенд не собирается
- Проверьте логи в GitHub Actions
- Убедитесь, что все зависимости установлены (`npm ci`)

## 🎉 Готово!

После успешного деплоя вы получите:
- ✅ Автоматический деплой при каждом push
- ✅ URL вашего фронтенда на Cloudflare Pages
- ✅ HTTPS из коробки
- ✅ CDN от Cloudflare

---

**Следующий шаг**: После деплоя фронтенда, обновите `REACT_APP_API_URL` на URL вашего бэкенда API.
