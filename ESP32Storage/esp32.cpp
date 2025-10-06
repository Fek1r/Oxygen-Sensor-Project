//Here I just save the code from the Arduino IDE so that other people can look at it and so that 
// I always know where to find the code.


#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include <Wire.h>
#include <SensirionI2cScd4x.h>

// WiFi Home
// const char* ssid = "2_2.4G";
// const char* password = "10021711";

//WiFi Victoria
// const char* ssid = "VICTORIA-WiFi";
// const char* password = "Students@VICTORIA!";

//WiFi Caffeine
// const char* ssid = "In Coffee We Trust";
// const char* password = "";

//Wifi
const char* ssid = "Dark Network";
const char* password = "111alah666!";



// Адрес сервера
const char* serverUrl = "http://10.221.84.53:5245/sensor/receive";

// LED пины
const int LED_GREEN = 12;   // Зеленый: 400-1000 ppm CO2
const int LED_YELLOW = 14;  // Желтый: 1001-1400 ppm CO2
const int LED_RED = 26;     // Красный: 1401-10000 ppm CO2
const int LED_WIFI = 4;     // Синий: статус WiFi
const int LED_SERVER = 18;  // Синий: статус подключения к серверу

SensirionI2cScd4x sensor;
int16_t error;

void setupLEDs() {
  pinMode(LED_GREEN, OUTPUT);
  pinMode(LED_YELLOW, OUTPUT);
  pinMode(LED_RED, OUTPUT);
  pinMode(LED_WIFI, OUTPUT);
  pinMode(LED_SERVER, OUTPUT);
  
  // Выключаем все LED при старте
  digitalWrite(LED_GREEN, LOW);
  digitalWrite(LED_YELLOW, LOW);
  digitalWrite(LED_RED, LOW);
  digitalWrite(LED_WIFI, LOW);
  digitalWrite(LED_SERVER, LOW);
}

void updateCO2LEDs(uint16_t co2) {
  // Выключаем все LED загрязнения
  digitalWrite(LED_GREEN, LOW);
  digitalWrite(LED_YELLOW, LOW);
  digitalWrite(LED_RED, LOW);
  
  // Включаем соответствующий LED
  if (co2 >= 400 && co2 <= 1000) {
    digitalWrite(LED_GREEN, HIGH);
    Serial.println("LED: Зеленый (низкое загрязнение)");
  } else if (co2 >= 1001 && co2 <= 1400) {
    digitalWrite(LED_YELLOW, HIGH);
    Serial.println("LED: Желтый (среднее загрязнение)");
  } else if (co2 >= 1401 && co2 <= 10000) {
    digitalWrite(LED_RED, HIGH);
    Serial.println("LED: Красный (высокое загрязнение)");
  }
}

void connectWiFi() {
  Serial.print("Подключение к WiFi: ");
  Serial.println(ssid);
  
  digitalWrite(LED_WIFI, LOW);  // Выключаем LED WiFi
  
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
    digitalWrite(LED_WIFI, HIGH);  // Включаем LED WiFi
    Serial.println(">>> WiFi успешно подключен! <<<");
    Serial.print("IP: ");
    Serial.println(WiFi.localIP());
    Serial.print("MAC: ");
    Serial.println(WiFi.macAddress());
    Serial.print("RSSI: ");
    Serial.print(WiFi.RSSI());
    Serial.println(" dBm");
  } else {
    digitalWrite(LED_WIFI, LOW);  // Выключаем LED WiFi
    Serial.println(">>> ОШИБКА: WiFi не подключен!");
  }
}

void setup() {
  Serial.begin(9600);
  delay(2000);
  Serial.println("\n\n=== ESP32 Start ===");
  
  setupLEDs();  // Инициализируем LED
  
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
    digitalWrite(LED_WIFI, LOW);  // Выключаем LED WiFi
    Serial.println(">>> WiFi отключен! Переподключение... <<<");
    connectWiFi();
    if (WiFi.status() != WL_CONNECTED) {
      Serial.println(">>> Не удалось подключиться! Ждем 15 секунд...");
      delay(15000);
      return;
    }
  } else {
    digitalWrite(LED_WIFI, HIGH);  // Включаем LED WiFi
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
  
  // Обновляем LED индикацию CO2
  updateCO2LEDs(co2);
  
  // Получим MAC ESP32
  String mac = WiFi.macAddress();
  Serial.print("MAC: ");
  Serial.println(mac);
  
  // Создаём JSON
  StaticJsonDocument<256> doc;
  doc["MAC"] = mac;
  doc["name"] = "ESP32 - #1";
  doc["temp"] = temperature;
  doc["hum"] = humidity;
  doc["co2"] = co2;
  
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
    digitalWrite(LED_SERVER, HIGH);  // Включаем LED сервера
    Serial.print("✓ УСПЕХ! Код ответа: ");
    Serial.println(httpResponseCode);
    Serial.print("Ответ сервера: ");
    Serial.println(http.getString());
  } else {
    digitalWrite(LED_SERVER, LOW);  // Выключаем LED сервера
    Serial.print("✗ ОШИБКА HTTP: ");
    Serial.println(httpResponseCode);
  }
  
  http.end();
  
  Serial.println("Ожидание 15 секунд...");
  Serial.println("==========================================\n");
  delay(15000);
}