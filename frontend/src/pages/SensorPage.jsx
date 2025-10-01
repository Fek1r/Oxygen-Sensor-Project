
import React, { useEffect, useState } from "react";
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
  const [data, setData] = useState([]);
  const [latest, setLatest] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch("http://192.168.2.49:5245/sensor/latest");
        if (!res.ok) throw new Error("Ошибка: " + res.status);
        const json = await res.json();

        const formatted = [
          {
            time: new Date().toLocaleTimeString(),
            temperature: json.temp,
            humidity: json.hum,
            co2: json.co2,
            mac: json.MAC
          }
        ];

        setData((prev) => [...prev.slice(-9), ...formatted]);
        setLatest(json);
      } catch (err) {
        console.error("Ошибка загрузки данных:", err);
      }
    };

    fetchData();
    const interval = setInterval(fetchData, 15000);
    return () => clearInterval(interval);
  }, []);

  return (
    <div className="app">
      <div className="sensor-page">
        <h1>Данные с датчика</h1>

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
    </div>
  );
};

export default SensorPage;