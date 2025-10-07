#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include <Wire.h>
#include <SensirionI2cScd4x.h>

//Wifi
const char* ssid = "Dark Network";
const char* password = "111alah666!";

// Адрес сервера
const char* serverUrl = "http://10.221.84.53:5245/sensor/receive";

// I2C пины
const int I2C_SDA = 21;
const int I2C_SCL = 22;

// LED пины
const int LED_GREEN = 12;
const int LED_YELLOW = 14;
const int LED_RED = 26;
const int LED_WIFI = 4;
const int LED_SERVER = 18;
const int BUZZER = 33;

SensirionI2cScd4x sensor;
int16_t error;

void setupLEDs() {
  pinMode(LED_GREEN, OUTPUT);
  pinMode(LED_YELLOW, OUTPUT);
  pinMode(LED_RED, OUTPUT);
  pinMode(LED_WIFI, OUTPUT);
  pinMode(LED_SERVER, OUTPUT);
  pinMode(BUZZER, OUTPUT);
  
  digitalWrite(LED_GREEN, LOW);
  digitalWrite(LED_YELLOW, LOW);
  digitalWrite(LED_RED, LOW);
  digitalWrite(LED_WIFI, LOW);
  digitalWrite(LED_SERVER, LOW);
  digitalWrite(BUZZER, LOW);
  
  Serial.println("✓ LED и пикалка инициализированы");
}

void updateCO2LEDs(uint16_t co2) {
  static unsigned long lastBeepTime = 0;   // время последнего срабатывания
  static bool buzzerActive = false;

  digitalWrite(LED_GREEN, LOW);
  digitalWrite(LED_YELLOW, LOW);
  digitalWrite(LED_RED, LOW);
  digitalWrite(BUZZER, LOW);  // Сначала выключаем
  
  if (co2 >= 400 && co2 <= 1099) {
    digitalWrite(LED_GREEN, HIGH);
    Serial.println("🟢 LED: Зеленый (низкое загрязнение)");
  } else if (co2 >= 1100 && co2 <= 1600) {
    digitalWrite(LED_YELLOW, HIGH);
    Serial.println("🟡 LED: Желтый (среднее загрязнение)");
  } else if (co2 >= 1601) {
    digitalWrite(LED_RED, HIGH);
    digitalWrite(BUZZER, HIGH);  // ВКЛЮЧАЕМ ПИКАЛКУ!
    Serial.println("🔴 LED: Красный (высокое загрязнение)");
    Serial.println("🔊 ПИКАЛКА ВКЛЮЧЕНА! CO2 >= 1601 ppm");
  }


  if (buzzerActive && millis() - lastBeepTime >= 1000) {
    digitalWrite(BUZZER, LOW);
    buzzerActive = false;
    Serial.println("🔇 ПИКАЛКА ВЫКЛЮЧЕНА после 1 секунд");
  }
}

void connectWiFi() {
  Serial.print("Подключение к WiFi: ");
  Serial.println(ssid);
  
  digitalWrite(LED_WIFI, LOW);
  WiFi.disconnect(true);
  delay(1000);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  
  int attempts = 0;
  while (WiFi.status() != WL_CONNECTED && attempts < 40) {
    delay(500);
    Serial.print(".");
    attempts++;
  }
  Serial.println();
  
  if (WiFi.status() == WL_CONNECTED) {
    digitalWrite(LED_WIFI, HIGH);
    Serial.println("✓ WiFi подключен!");
    Serial.print("IP: ");
    Serial.println(WiFi.localIP());
  } else {
    Serial.println("✗ WiFi не подключен!");
  }
}

void setup() {
  Serial.begin(115200);
  delay(2000);
  Serial.println("\n\n=== ESP32 CO2 Monitor Start ===");
  
  setupLEDs();
  
  // ТЕСТ ПИКАЛКИ ПРИ СТАРТЕ
  Serial.println("\n>>> ТЕСТ ПИКАЛКИ <<<");
  digitalWrite(BUZZER, HIGH);
  delay(500);
  digitalWrite(BUZZER, LOW);
  delay(500);
  Serial.println("✓ Тест пикалки завершен (должна была пищать 1 раза)\n");
  
  connectWiFi();
  
  Serial.println("\nИнициализация I2C и датчика...");
  Serial.printf("SDA: GPIO%d, SCL: GPIO%d\n", I2C_SDA, I2C_SCL);
  
  Wire.begin(I2C_SDA, I2C_SCL);
  Wire.setClock(50000);
  delay(500);
  
  // Сканирование I2C
  Serial.println("Сканирование I2C шины...");
  byte address;
  int nDevices = 0;
  
  for(address = 1; address < 127; address++) {
    Wire.beginTransmission(address);
    byte error = Wire.endTransmission();
    
    if (error == 0) {
      Serial.print("  ✓ Устройство найдено на адресе: 0x");
      if (address < 16) Serial.print("0");
      Serial.println(address, HEX);
      nDevices++;
    }
  }
  
  if (nDevices == 0) {
    Serial.println("\n✗✗✗ ОШИБКА: I2C устройства не найдены! ✗✗✗");
    Serial.println("Проверьте подключение датчика:");
    Serial.println("  - SDA датчика → GPIO21 ESP32");
    Serial.println("  - SCL датчика → GPIO22 ESP32");
    Serial.println("  - VCC датчика → 3.3V или 5V");
    Serial.println("  - GND датчика → GND");
  } else {
    Serial.printf("✓ Найдено I2C устройств: %d\n", nDevices);
  }
  
  sensor.begin(Wire, SCD41_I2C_ADDR_62);
  delay(500);
  sensor.wakeUp();
  delay(500);
  
  Serial.println("✓ Датчик готов!");
  Serial.println("=================================\n");
}

void loop() {
  Serial.println("╔════════════════════════════════╗");
  Serial.println("║   Начало цикла измерения      ║");
  Serial.println("╚════════════════════════════════╝");
  
  uint16_t co2;
  float temperature;
  float humidity;
  
  Serial.println("📊 Чтение данных с датчика...");
  error = sensor.measureAndReadSingleShot(co2, temperature, humidity);
  
  if (error != 0) {
    Serial.print("✗ ОШИБКА чтения датчика, код: ");
    Serial.println(error);
    Serial.println("⚠ Попытка сброса I2C...");
    
    Wire.end();
    delay(1000);
    Wire.begin(I2C_SDA, I2C_SCL);
    Wire.setClock(50000);
    delay(1000);
    sensor.begin(Wire, SCD41_I2C_ADDR_62);
    sensor.wakeUp();
    delay(1000);
    
    Serial.println("Ожидание 10 секунд...\n");
    delay(10000);
    return;
  }
  
  Serial.println("\n📈 РЕЗУЛЬТАТЫ ИЗМЕРЕНИЙ:");
  Serial.printf("  🌡️  Температура: %.2f °C\n", temperature);
  Serial.printf("  💧 Влажность: %.2f %%\n", humidity);
  Serial.printf("  🫁 CO₂: %u ppm\n\n", co2);
  
  updateCO2LEDs(co2);
  
  // WiFi и отправка данных
  bool wifiConnected = (WiFi.status() == WL_CONNECTED);
  
  if (!wifiConnected) {
    digitalWrite(LED_WIFI, LOW);
    Serial.println("⚠ WiFi отключен, переподключение...");
    connectWiFi();
    wifiConnected = (WiFi.status() == WL_CONNECTED);
  } else {
    digitalWrite(LED_WIFI, HIGH);
  }
  
  if (wifiConnected) {
    String mac = WiFi.macAddress();
    
    StaticJsonDocument<256> doc;
    doc["MAC"] = mac;
    doc["name"] = "ESP32 - #1";
    doc["temp"] = temperature;
    doc["hum"] = humidity;
    doc["co2"] = co2;
    
    String jsonString;
    serializeJson(doc, jsonString);
    Serial.print("📤 Отправка: ");
    Serial.println(jsonString);
    
    HTTPClient http;
    http.begin(serverUrl);
    http.addHeader("Content-Type", "application/json");
    http.setTimeout(10000);
    
    int httpResponseCode = http.POST(jsonString);
    
    if (httpResponseCode > 0) {
      digitalWrite(LED_SERVER, HIGH);
      Serial.print("✓ Успешно отправлено! Код: ");
      Serial.println(httpResponseCode);
    } else {
      digitalWrite(LED_SERVER, LOW);
      Serial.print("✗ Ошибка HTTP: ");
      Serial.println(httpResponseCode);
    }
    
    http.end();
  } else {
    digitalWrite(LED_SERVER, LOW);
    Serial.println("⚠ Данные не отправлены - нет WiFi");
  }
  
  Serial.println("\n⏳ Ожидание 5 секунд...");
  Serial.println("════════════════════════════════\n");
  delay(5000);
}