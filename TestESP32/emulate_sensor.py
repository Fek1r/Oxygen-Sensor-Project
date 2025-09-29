import requests
import random
import time
import json

# Адрес твоего backend-сервера
SERVER_URL = "http://localhost:5245/sensor/receive"  # ⚠️ замени путь на твой контроллер

def generate_sensor_data():
    """Генерируем случайные данные, имитирующие датчик ESP32"""
    return {
        "MAC": "3A:7F:1C:9B:4D:E2",
        "temp": round(random.uniform(20, 35), 1),
        "hum": round(random.uniform(40, 70), 1)
    }

def send_data():
    """Отправляем данные каждые 5 секунд"""
    while True:
        data = generate_sensor_data()
        try:
            response = requests.post(SERVER_URL, json=data)
            if response.ok:
                print(f"✅ Sent: {json.dumps(data)} | Response: {response.status_code}")
            else:
                print(f"❌ Server error {response.status_code} | {response.text}")
        except Exception as e:
            print(f"⚠️ Error sending data: {e}")
        time.sleep(5)  # пауза между запросами

if __name__ == "__main__":
    send_data()
