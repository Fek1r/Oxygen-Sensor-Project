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
        const res = await fetch("http://localhost:5245/sensor/latest");
        if (!res.ok) throw new Error("Ошибка: " + res.status);
        const json = await res.json();

        // Превращаем один объект в массив для графика
        const formatted = [
          {
            time: new Date().toLocaleTimeString(), // текущее время
            temperature: json.temp,
            humidity: json.hum,
            mac: json.MAC
          }
        ];

        // Сохраняем последние 10 точек
        setData((prev) => [...prev.slice(-9), ...formatted]);
        setLatest(json);
      } catch (err) {
        console.error("Ошибка загрузки данных:", err);
      }
    };

    fetchData();
    const interval = setInterval(fetchData, 2000); // обновление каждые 2 секунд
    return () => clearInterval(interval);
  }, []);

  return (
    <div className="app">
      <div className="sensor-page">
        <h1>Данные с датчика</h1>

        <div className="chart-container">
          <ResponsiveContainer width="100%" height={300}>
            <LineChart data={data}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="time" />
              <YAxis />
              <Tooltip />
              <Legend />
              <Line
                type="monotone"
                dataKey="temperature"
                stroke="#0077b6"
                name="Температура (°C)"
              />
              <Line
                type="monotone"
                dataKey="humidity"
                stroke="#ff7f0e"
                name="Влажность (%)"
              />
            </LineChart>
          </ResponsiveContainer>
        </div>

        {latest && (
          <div className="latest-data">
            <p>
              <strong>MAC:</strong> {latest.MAC}
            </p>
            <p>
              <strong>Температура:</strong> {latest.temp} °C
            </p>
            <p>
              <strong>Влажность:</strong> {latest.hum} %
            </p>
          </div>
        )}
      </div>
    </div>
  );
};

export default SensorPage;
