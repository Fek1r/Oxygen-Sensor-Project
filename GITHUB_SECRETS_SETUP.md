# Настройка GitHub Secrets

## Ваши данные Cloudflare

- **API Token**: `De3FKowMsH1z86SQsba4c609CrtvQBorlkW0_aa1`
- **Account ID**: `aa2f7fc5de004e56e4c0189edbdd6ed7`
- **Tunnel Token**: `eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9`

## Инструкция по добавлению секретов в GitHub

### Шаг 1: Откройте настройки репозитория

1. Зайдите в ваш GitHub репозиторий
2. Нажмите на вкладку **Settings** (вверху)
3. В левом меню найдите **Secrets and variables** → **Actions**
4. Нажмите **New repository secret**

### Шаг 2: Добавьте секреты

Добавьте следующие секреты (нажмите "New repository secret" для каждого):

#### 1. CLOUDFLARE_API_TOKEN
- **Name**: `CLOUDFLARE_API_TOKEN`
- **Secret**: `De3FKowMsH1z86SQsba4c609CrtvQBorlkW0_aa1`
- Нажмите **Add secret**

#### 2. CLOUDFLARE_ACCOUNT_ID
- **Name**: `CLOUDFLARE_ACCOUNT_ID`
- **Secret**: `aa2f7fc5de004e56e4c0189edbdd6ed7`
- Нажмите **Add secret**

#### 3. CLOUDFLARE_TUNNEL_TOKEN
- **Name**: `CLOUDFLARE_TUNNEL_TOKEN`
- **Secret**: `eyJhIjoiYWEyZjdmYzVkZTAwNGU1NmU0YzAxODllZGJkZDZlZDciLCJ0IjoiODU5NjFiM2ItYjQ4Mi00NTU4LWFjNmUtZTFmNDA5NDhiMzcyIiwicyI6Ik1qUm1OVEV4TVRrdE56WTBZaTAwT0dJNExXSmxNbUV0T0RBMFpUVXdPVEJsTURSayJ9`
- Нажмите **Add secret**

#### 4. REACT_APP_API_URL (для фронтенда)
- **Name**: `REACT_APP_API_URL`
- **Secret**: `http://localhost:5245` (пока используем локальный, потом замените на URL вашего API)
- Нажмите **Add secret**

#### 5. POSTGRES_CONNECTION_STRING (опционально, для бэкенда)
- **Name**: `POSTGRES_CONNECTION_STRING`
- **Secret**: Ваша строка подключения к PostgreSQL (например: `Host=hostname;Port=5432;Database=dbname;Username=user;Password=pass`)
- Нажмите **Add secret**

### Шаг 3: Проверка

После добавления всех секретов, вы должны увидеть список:
- ✅ CLOUDFLARE_API_TOKEN
- ✅ CLOUDFLARE_ACCOUNT_ID
- ✅ CLOUDFLARE_TUNNEL_TOKEN
- ✅ REACT_APP_API_URL
- ✅ POSTGRES_CONNECTION_STRING (если добавили)

## Что дальше?

После добавления секретов:

1. Закоммитьте и запушьте изменения:
   ```bash
   git add .
   git commit -m "Add frontend deployment workflow"
   git push origin main
   ```

2. GitHub Actions автоматически запустится и задеплоит фронтенд на Cloudflare Pages

3. Проверьте статус в **Actions** вкладке вашего репозитория

4. После успешного деплоя, вы получите URL вида: `https://oxygen-sensor-frontend.pages.dev`

## Важно!

⚠️ **Не коммитьте эти токены в код!** Они уже добавлены в секреты GitHub и будут использоваться безопасно.
