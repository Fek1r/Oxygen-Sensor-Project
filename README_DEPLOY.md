# ⚠️ ВАЖНО: Создайте проект вручную перед деплоем

## Проблема
GitHub Actions не может автоматически создать Cloudflare Pages проект через API (ограничения прав доступа).

## ✅ Решение: Создайте проект вручную (2 минуты)

### Шаг 1: Откройте Cloudflare Dashboard
👉 https://dash.cloudflare.com/

### Шаг 2: Создайте проект
1. В левом меню: **Workers & Pages** → **Pages**
2. Нажмите **Create a project**
3. Выберите **Upload assets** (или **Connect to Git**)
4. **Project name**: `oxygen-sensor-frontend`
5. Нажмите **Create project**

### Шаг 3: Готово!
После создания проекта, GitHub Actions workflow будет автоматически деплоить ваш фронтенд при каждом push.

---

## После создания проекта

Просто запушьте изменения:

```bash
git add .
git commit -m "Deploy frontend"
git push origin feature/DeploDocker
```

Workflow автоматически:
- ✅ Соберет React приложение
- ✅ Задеплоит на Cloudflare Pages
- ✅ Даст вам URL вида: `https://oxygen-sensor-frontend.pages.dev`

---

## Проверка

После деплоя проверьте:
1. Вкладка **Actions** в GitHub - должен быть зеленый статус ✅
2. Cloudflare Dashboard → Pages → `oxygen-sensor-frontend` - должен быть деплой
3. Откройте URL проекта в браузере
