import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer
} from "recharts";
import "./SensorPage.css";

const SensorPage = () => {
  const { mac } = useParams();
  const [data, setData] = useState([]);
  const [latest, setLatest] = useState(null);
  const [error, setError] = useState(null);
  const [period, setPeriod] = useState("live"); // hour, day, week, live
  const [loading, setLoading] = useState(false);
  
  const baseUrl = process.env.REACT_APP_API_URL;

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        
        // ✅ Используем новый эндпоинт /sensor/history
        const params = new URLSearchParams({ period });
        if (mac) params.append("mac", mac);
        
        const res = await fetch(`${baseUrl}/sensor/history?${params}`);
        if (!res.ok) throw new Error("Ошибка: " + res.status);
        
        const json = await res.json();
        
        // Форматируем данные для графика
        const formatted = json.data.map(item => ({
          time: new Date(item.timestamp).toLocaleTimeString("ru-RU", {
            hour: "2-digit",
            minute: "2-digit"
          }),
          fullTime: new Date(item.timestamp).toLocaleString("ru-RU"),
          temperature: item.temp,
          humidity: item.hum,
          co2: item.co2,
          mac: item.mac,
          name: item.name
        }));
        
        setData(formatted);
        
        // Последнее значение для карточки
        if (formatted.length > 0) {
          setLatest({
            mac: formatted[formatted.length - 1].mac,
            name: formatted[formatted.length - 1].name,
            temp: formatted[formatted.length - 1].temperature,
            hum: formatted[formatted.length - 1].humidity,
            co2: formatted[formatted.length - 1].co2
          });
        }
        
        setError(null);
        setLoading(false);
      } catch (err) {
        console.error("Ошибка загрузки данных:", err);
        setError(err.message);
        setLoading(false);
      }
    };

    fetchData();
    
    // Автообновление только в режиме live
    let interval;
    if (period === "live") {
      interval = setInterval(fetchData, 15000); // каждые 15 сек
    }
    
    return () => {
      if (interval) clearInterval(interval);
    };
  }, [mac, period, baseUrl]);

  // ✅ Скачивание CSV
  const handleDownloadCsv = () => {
    const params = new URLSearchParams({ period });
    if (mac) params.append("mac", mac);
    
    const url = `${baseUrl}/sensor/export/csv?${params}`;
    window.open(url, "_blank");
  };

  if (error) {
    return (
      <div className="sensor-page">
        <h1>Ошибка загрузки данных</h1>
        <p style={{ color: "red" }}>{error}</p>
      </div>
    );
  }

  return (
    <div className="sensor-page">
      <div className="header-controls">
        <h1>
          {mac
            ? `Данные устройства ${latest?.name || mac}`
            : "Мониторинг датчиков"}
        </h1>
        
        {/* ✅ Кнопки выбора периода */}
        <div className="time-filters">
          <button 
            className={period === "live" ? "active" : ""}
            onClick={() => setPeriod("live")}
          >
            🔴 Реал-тайм
          </button>
          <button 
            className={period === "hour" ? "active" : ""}
            onClick={() => setPeriod("hour")}
          >
            📊 Час
          </button>
          <button 
            className={period === "day" ? "active" : ""}
            onClick={() => setPeriod("day")}
          >
            📅 День
          </button>
          <button 
            className={period === "week" ? "active" : ""}
            onClick={() => setPeriod("week")}
          >
            📆 Неделя
          </button>
        </div>
      </div>

      {loading && <div className="loading">Загрузка данных...</div>}

      {data.length === 0 && !loading && (
        <div className="no-data">Нет данных за выбранный период</div>
      )}

      {data.length > 0 && (
        <>
          <div className="chart-container">
            <ResponsiveContainer width="100%" height={400}>
              <LineChart data={data}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis 
                  dataKey="time" 
                  angle={-45}
                  textAnchor="end"
                  height={80}
                />
                
                {/* Левая ось Y для температуры и влажности */}
                <YAxis
                  yAxisId="left"
                  label={{ value: '°C / %', angle: -90, position: 'insideLeft' }}
                  domain={[0, 100]}
                />
                
                {/* Правая ось Y для CO₂ */}
                <YAxis
                  yAxisId="right"
                  orientation="right"
                  label={{ value: 'CO₂ (ppm)', angle: 90, position: 'insideRight' }}
                  domain={[400, 5000]}
                />
                
                <Tooltip 
                  content={({ payload }) => {
                    if (!payload || !payload.length) return null;
                    const data = payload[0].payload;
                    return (
                      <div className="custom-tooltip">
                        <p><strong>{data.fullTime}</strong></p>
                        <p>🌡️ Температура: {data.temperature?.toFixed(1)} °C</p>
                        <p>💧 Влажность: {data.humidity?.toFixed(1)} %</p>
                        <p>🌫️ CO₂: {data.co2} ppm</p>
                      </div>
                    );
                  }}
                />
                
                <Legend />
                
                <Line
                  yAxisId="left"
                  type="monotone"
                  dataKey="temperature"
                  stroke="#0077b6"
                  strokeWidth={2}
                  name="Температура (°C)"
                  dot={{ r: 3 }}
                />
                <Line
                  yAxisId="left"
                  type="monotone"
                  dataKey="humidity"
                  stroke="#ff7f0e"
                  strokeWidth={2}
                  name="Влажность (%)"
                  dot={{ r: 3 }}
                />
                <Line
                  yAxisId="right"
                  type="monotone"
                  dataKey="co2"
                  stroke="#2ca02c"
                  strokeWidth={2}
                  name="CO₂ (ppm)"
                  dot={{ r: 3 }}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>

          {latest && (
            <div className="latest-data">
              <h3>Последние показания</h3>
              <p><strong>MAC:</strong> {latest.mac || 'Не указан'}</p>
              {latest.name && <p><strong>Название:</strong> {latest.name}</p>}
              <p><strong>Температура:</strong> {Math.round(latest.temp)} °C</p>
              <p><strong>Влажность:</strong> {Math.round(latest.hum)} %</p>
              <p><strong>CO₂:</strong> {latest.co2} ppm</p>
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default SensorPage;