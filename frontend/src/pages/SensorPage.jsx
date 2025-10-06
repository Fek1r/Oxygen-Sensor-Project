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
  const { mac } = useParams(); // ✅ Получаем MAC из URL
  const [data, setData] = useState([]);
  const [latest, setLatest] = useState(null);
  const [error, setError] = useState(null);
  const baseUrl = process.env.REACT_APP_API_URL;


  useEffect(() => {
    const fetchData = async () => {
      try {
        // ✅ Если указан MAC, загружаем данные конкретного устройства
        const endpoint = mac 
        ? `${baseUrl}/sensor/device/${mac}`
        : `${baseUrl}/sensor/latest`;
        
        const res = await fetch(endpoint);
        if (!res.ok) throw new Error("Ошибка: " + res.status);
        const json = await res.json();

        const formatted = {
          time: new Date().toLocaleTimeString(),
          temperature: json.temp,
          humidity: json.hum,
          co2: json.co2,
          mac: json.mac
        };

        setData((prev) => [...prev.slice(-9), formatted]);
        setLatest(json);
        setError(null);
      } catch (err) {
        console.error("Ошибка загрузки данных:", err);
        setError(err.message);
      }
    };

    fetchData();
    const interval = setInterval(fetchData, 15000);
    return () => clearInterval(interval);
  }, [mac]); // ✅ Перезагружаем при смене MAC

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
      <h1>
        {mac 
          ? `Данные устройства ${latest?.name || mac}` 
          : "Данные с датчика"}
      </h1>

      <div className="chart-container">
        <ResponsiveContainer width="100%" height={400}>
          <LineChart data={data}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="time" />
            
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
            
            <Tooltip />
            <Legend />
            
            <Line
              yAxisId="left"
              type="monotone"
              dataKey="temperature"
              stroke="#0077b6"
              strokeWidth={2}
              name="Температура (°C)"
              dot={{ r: 4 }}
            />
            <Line
              yAxisId="left"
              type="monotone"
              dataKey="humidity"
              stroke="#ff7f0e"
              strokeWidth={2}
              name="Влажность (%)"
              dot={{ r: 4 }}
            />
            <Line
              yAxisId="right"
              type="monotone"
              dataKey="co2"
              stroke="#2ca02c"
              strokeWidth={2}
              name="CO₂ (ppm)"
              dot={{ r: 4 }}
            />
          </LineChart>
        </ResponsiveContainer>
      </div>

      {latest && (
        <div className="latest-data">
          <p>
            <strong>MAC:</strong> {latest.mac || 'Не указан'}
          </p>
          {latest.name && (
            <p>
              <strong>Название:</strong> {latest.name}
            </p>
          )}
          <p>
            <strong>Температура:</strong> {Math.round(latest.temp)} °C
          </p>
          <p>
            <strong>Влажность:</strong> {Math.round(latest.hum)} %
          </p>
          <p>
            <strong>CO₂:</strong> {latest.co2} ppm
          </p>
        </div>
      )}
    </div>
  );
};

export default SensorPage;