# Oxygen Monitoring System

## 📌 Архитектура проекта

Система реализована по принципу **MVC** и с использованием **микросервисов**.  

### Общая схема системы

```text
+------------------+          +-------------------+         +------------------+
|   ESP (Arduino)  |   REST   |   API Gateway     |  REST   |  Frontend (React)|
| [Oxygen Sensor]  +--------->+  - Auth routing   +<------->+  (Login, Dashboard|
|  JSON data       |          |  - Sensor routing |         |  Device Mgmt UI) |
+------------------+          +---------+---------+         +------------------+
                                       |
                                       v
                           +-----------------------+
                           |  Auth Service (C#)    |
                           |  - Users, JWT tokens  |
                           +-----------------------+
                                       |
                           +-----------------------+
                           | Sensor Service (C#)   |
                           | - Save oxygen data    |
                           | - Manage devices      |
                           +-----------------------+
                                       |
                           +-----------------------+
                           |   Database (SQL)      |
                           | users, devices, data  |
                           +-----------------------+
+-------------------+
|      Users        |
+-------------------+
| user_id (PK)      |
| username          |
| password_hash     |
| role              |
| created_at        |
+-------------------+

+-------------------+
|     Devices       |
+-------------------+
| device_id (PK)    |
| device_name       |
| location          |
| owner_id (FK) -> Users.user_id
| created_at        |
+-------------------+

+-------------------+
|   Measurements    |
+-------------------+
| measure_id (PK)   |
| device_id (FK) -> Devices.device_id
| oxygen_level      |
| timestamp         |
+-------------------+

1. Пользователь вводит логин и пароль.  
2. Запрос → POST /api/auth/login  
3. Auth Service проверяет данные в таблице Users.  
4. При успехе возвращается JWT токен.  
5. Все дальнейшие запросы (например, к Sensor Service) идут с заголовком:  

Authorization: Bearer <jwt_token>

+-------------------------------------+
|             Login Form              |
+-------------------------------------+
|  Username: [______________]         |
|  Password: [______________]         |
|                                     |
|  [ Login Button ]                   |
|                                     |
|  -> POST /api/auth/login            |
|     (возврат JWT токена)            |
+-------------------------------------+

1. ESP считывает данные с датчика кислорода.  
2. Отправляет JSON по REST → Sensor Service (C#).  
3. Данные сохраняются в SQL БД.  
4. Пользователь логинится через Auth Service (C#).  
5. React UI показывает список устройств и их показатели.  
