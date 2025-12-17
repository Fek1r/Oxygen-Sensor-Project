# Настройка Cloudflare Pages проекта

## Проблема
Проект `oxygen-sensor-frontend` не существует в Cloudflare Pages.

## Решение 1: Создать проект вручную (рекомендуется)

1. Зайдите в [Cloudflare Dashboard](https://dash.cloudflare.com/)
2. Перейдите в **Workers & Pages** → **Pages**
3. Нажмите **Create a project**
4. Выберите **Upload assets** (или **Connect to Git** если хотите)
5. Назовите проект: `oxygen-sensor-frontend`
6. Нажмите **Create project**

После создания проекта, GitHub Actions workflow будет работать автоматически.

## Решение 2: Использовать обновленный workflow

Я обновил workflow, чтобы он автоматически создавал проект, если его нет. Просто запушьте изменения:

```bash
git add .
git commit -m "Add auto-create Cloudflare Pages project"
git push origin feature/DeploDocker
```

## Решение 3: Использовать Wrangler CLI

Альтернативный способ - использовать Wrangler для создания проекта:

```bash
npm install -g wrangler
wrangler pages project create oxygen-sensor-frontend
```

## Проверка

После создания проекта, проверьте что он существует:
- Зайдите в Cloudflare Dashboard → Workers & Pages → Pages
- Убедитесь, что проект `oxygen-sensor-frontend` виден в списке
