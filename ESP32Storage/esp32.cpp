//Here I just save the code from the Arduino IDE so that other people can look at it and so that 
// I always know where to find the code.


#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include <Wire.h>
#include <SensirionI2cScd4x.h>

// WiFi настройки
const char* ssid = "2_2.4G";
const char* password = "10021711";

// Адрес сервера
const char* serverUrl = "http://192.168.2.49:5245/sensor/receive";

SensirionI2cScd4x sensor;
int16_t error;

void connectWiFi() {
  Serial.print("Подключение к WiFi: ");
  Serial.println(ssid);
  
  WiFi.disconnect(true);
  delay(1000);
  WiFi.mode(WIFI_OFF);
  delay(1000);
  WiFi.mode(WIFI_STA);
  delay(1000);
  WiFi.begin(ssid, password);
  
  int attempts = 0;
  while (WiFi.status() != WL_CONNECTED && attempts < 40) {
    delay(500);
    Serial.print(".");
    attempts++;
  }
  Serial.println();
  
  if (WiFi.status() == WL_CONNECTED) {
    Serial.println(">>> WiFi успешно подключен! <<<");
    Serial.print("IP: ");
    Serial.println(WiFi.localIP());
    Serial.print("MAC: ");
    Serial.println(WiFi.macAddress());
    Serial.print("RSSI: ");
    Serial.print(WiFi.RSSI());
    Serial.println(" dBm");
  } else {
    Serial.println(">>> ОШИБКА: WiFi не подключен!");
  }
}

void setup() {
  Serial.begin(9600);
  delay(2000);
  Serial.println("\n\n=== ESP32 Start ===");
  
  connectWiFi();
  
  // I2C
  Serial.println("Инициализация датчика...");
  Wire.begin();
  sensor.begin(Wire, SCD41_I2C_ADDR_62);
  delay(100);
  sensor.wakeUp();
  Serial.println("Датчик готов!");
  Serial.println("===================\n");
}

void loop() {
  Serial.println("--- Начало цикла измерения ---");
  
  if (WiFi.status() != WL_CONNECTED) {
    Serial.println(">>> WiFi отключен! Переподключение... <<<");
    connectWiFi();
    if (WiFi.status() != WL_CONNECTED) {
      Serial.println(">>> Не удалось подключиться! Ждем 15 секунд...");
      delay(15000);
      return;
    }
  }
  
  uint16_t co2;
  float temperature;
  float humidity;
  
  Serial.println("Чтение данных с датчика...");
  error = sensor.measureAndReadSingleShot(co2, temperature, humidity);
  
  if (error != 0) {
    Serial.print("ОШИБКА чтения датчика, код: ");
    Serial.println(error);
    delay(5000);
    return;
  }
  
  Serial.printf("Температура: %.2f °C\n", temperature);
  Serial.printf("Влажность: %.2f %%\n", humidity);
  Serial.printf("CO₂: %u ppm\n", co2);
  
  // Получим MAC ESP32
  String mac = WiFi.macAddress();
  Serial.print("MAC: ");
  Serial.println(mac);
  
  // Создаём JSON
  StaticJsonDocument<256> doc;
  doc["MAC"] = mac;
  doc["temp"] = temperature;
  doc["hum"] = humidity;
  doc["co2"] = co2;   // ✅ Добавлено CO₂
  
  String jsonString;
  serializeJson(doc, jsonString);
  Serial.print("JSON: ");
  Serial.println(jsonString);
  
  // Отправляем POST-запрос
  HTTPClient http;
  http.begin(serverUrl);
  http.addHeader("Content-Type", "application/json");
  http.setTimeout(10000);
  
  int httpResponseCode = http.POST(jsonString);
  
  if (httpResponseCode > 0) {
    Serial.print("✓ УСПЕХ! Код ответа: ");
    Serial.println(httpResponseCode);
    Serial.print("Ответ сервера: ");
    Serial.println(http.getString());
  } else {
    Serial.print("✗ ОШИБКА HTTP: ");
    Serial.println(httpResponseCode);
  }
  
  http.end();
  
  Serial.println("Ожидание 15 секунд...");
  Serial.println("==========================================\n");
  delay(15000);
}
