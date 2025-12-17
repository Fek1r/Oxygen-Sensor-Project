# Создание Cloudflare Pages проекта вручную

## ⚠️ Важно: Проект нужно создать вручную

GitHub Actions не может автоматически создать проект, если у API токена нет нужных прав или проект не существует.

## 📋 Пошаговая инструкция

### Шаг 1: Откройте Cloudflare Dashboard
1. Зайдите на https://dash.cloudflare.com/
2. Войдите в свой аккаунт

### Шаг 2: Создайте проект
1. В левом меню найдите **Workers & Pages**
2. Нажмите на **Pages**
3. Нажмите кнопку **Create a project**
4. Выберите **Upload assets** (или **Connect to Git** если хотите)
5. В поле **Project name** введите: `oxygen-sensor-frontend`
6. Нажмите **Create project**

### Шаг 3: Проверка
После создания проекта:
- Проект появится в списке проектов
- Вы можете закрыть страницу - проект уже создан
- GitHub Actions workflow теперь будет работать

### Шаг 4: Запустите деплой снова
После создания проекта, запушьте изменения или перезапустите workflow:

```bash
git push origin feature/DeploDocker
```

Или в GitHub:
1. Откройте вкладку **Actions**
2. Найдите последний запуск workflow
3. Нажмите **Re-run jobs**

## ✅ Готово!

После создания проекта, все последующие деплои будут работать автоматически.

## 🔍 Альтернатива: Использовать Wrangler CLI

Если предпочитаете командную строку:

```bash
npm install -g wrangler
wrangler login
wrangler pages project create oxygen-sensor-frontend
```
