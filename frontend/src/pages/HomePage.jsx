import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import SensorCard from "../components/SensorCard";
import "./HomePage.css";

const HomePage = () => {
  const navigate = useNavigate();
  const [devices, setDevices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Загрузка списка устройств с сервера
  useEffect(() => {
    const fetchDevices = async () => {
      try {
        const response = await fetch("http://10.51.0.120:5245/sensor/devices");
        if (!response.ok) {
          throw new Error("Ошибка загрузки устройств");
        }
        const data = await response.json();
        setDevices(data);
        setError(null);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchDevices();
    
    // ✅ Обновление каждые 10 секунд для актуального статуса
    const interval = setInterval(fetchDevices, 10000);
    return () => clearInterval(interval);
  }, []);

  if (loading) {
    return (
      <div className="homepage">
        <h1>Загрузка устройств...</h1>
      </div>
    );
  }

  if (error) {
    return (
      <div className="homepage">
        <h1>Ошибка</h1>
        <p style={{ color: "red" }}>{error}</p>
      </div>
    );
  }

  // ✅ Разделяем на онлайн и оффлайн устройства
  const onlineDevices = devices.filter(d => d.isOnline);
  const offlineDevices = devices.filter(d => !d.isOnline);

  return (
    <div className="homepage">
      <h1>Добро пожаловать!</h1>
      <p>Здесь вы можете выбрать датчик для мониторинга.</p>
      
      {devices.length === 0 ? (
        <p className="no-devices">
          Устройства не найдены. Убедитесь, что ESP32 отправляет данные на сервер.
        </p>
      ) : (
        <>
          {/* ✅ Онлайн устройства */}
          {onlineDevices.length > 0 && (
            <>
              <h2 className="section-title">
                🟢 Активные устройства ({onlineDevices.length})
              </h2>
              <div className="sensor-list">
                {onlineDevices.map((device) => (
                  <SensorCard 
                    key={device.mac}
                    title={device.name || `Устройство ${device.mac}`}
                    description={`MAC: ${device.mac}`}
                    extraInfo={`CO₂: ${device.co2} ppm | Темп: ${device.temp.toFixed(1)}°C`}
                    isOnline={true}
                    onSelect={() => navigate(`/sensor/${device.mac}`)} 
                  />
                ))}
              </div>
            </>
          )}

          {/* ✅ Оффлайн устройства */}
          {offlineDevices.length > 0 && (
            <>
              <h2 className="section-title offline-section">
                🔴 Неактивные устройства ({offlineDevices.length})
              </h2>
              <div className="sensor-list">
                {offlineDevices.map((device) => (
                  <SensorCard 
                    key={device.mac}
                    title={device.name || `Устройство ${device.mac}`}
                    description={`MAC: ${device.mac}`}
                    extraInfo={`Последние данные: CO₂: ${device.co2} ppm`}
                    isOnline={false}
                    onSelect={() => navigate(`/sensor/${device.mac}`)} 
                  />
                ))}
              </div>
            </>
          )}
        </>
      )}
    </div>
  );
};

export default HomePage;