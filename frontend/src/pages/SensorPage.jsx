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
        
        const params = new URLSearchParams({ period });
        if (mac) params.append("mac", mac);
        
        const res = await fetch(`${baseUrl}/sensor/history?${params}`);
        if (!res.ok) throw new Error("Error: " + res.status);
        
        const json = await res.json();
        
        // Format data for the chart (using US locale for time)
        const formatted = json.data.map(item => ({
          time: new Date(item.timestamp).toLocaleTimeString("en-US", {
            hour: "2-digit",
            minute: "2-digit",
            hour12: false // Можно оставить true, если нужен формат AM/PM
          }),
          fullTime: new Date(item.timestamp).toLocaleString("en-US"),
          temperature: item.temp,
          humidity: item.hum,
          co2: item.co2,
          mac: item.mac,
          name: item.name
        }));
        
        setData(formatted);
        
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
        console.error("Data loading error:", err);
        setError(err.message);
        setLoading(false);
      }
    };

    fetchData();
    
    let interval;
    if (period === "live") {
      interval = setInterval(fetchData, 15000);
    }
    
    return () => {
      if (interval) clearInterval(interval);
    };
  }, [mac, period, baseUrl]);

  const handleDownloadCsv = () => {
    const params = new URLSearchParams({ period });
    if (mac) params.append("mac", mac);
    
    const url = `${baseUrl}/sensor/export/csv?${params}`;
    window.open(url, "_blank");
  };

  if (error) {
    return (
      <div className="sensor-page">
        <h1>Data Loading Error</h1>
        <p style={{ color: "red" }}>{error}</p>
      </div>
    );
  }

  return (
    <div className="sensor-page">
      <div className="header-controls">
        <h1>
          {mac
            ? `Data for ${latest?.name || mac}`
            : "Sensor Monitoring"}
        </h1>
        
        <div className="time-filters">
          <button 
            className={period === "live" ? "active" : ""}
            onClick={() => setPeriod("live")}
          >
            🔴 Live
          </button>
          <button 
            className={period === "hour" ? "active" : ""}
            onClick={() => setPeriod("hour")}
          >
            📊 Hour
          </button>
          <button 
            className={period === "day" ? "active" : ""}
            onClick={() => setPeriod("day")}
          >
            📅 Day
          </button>
          <button 
            className={period === "week" ? "active" : ""}
            onClick={() => setPeriod("week")}
          >
            📆 Week
          </button>
        </div>
      </div>

      {loading && <div className="loading">Loading data...</div>}

      {data.length === 0 && !loading && (
        <div className="no-data">No data found for the selected period</div>
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
                
                <YAxis
                  yAxisId="left"
                  label={{ value: '°C / %', angle: -90, position: 'insideLeft' }}
                  domain={[0, 100]}
                />
                
                <YAxis
                  yAxisId="right"
                  orientation="right"
                  label={{ value: 'CO₂ (ppm)', angle: 90, position: 'insideRight' }}
                  domain={[400, 5000]}
                />
                
                <Tooltip 
                  content={({ payload }) => {
                    if (!payload || !payload.length) return null;
                    const itemData = payload[0].payload;
                    return (
                      <div className="custom-tooltip">
                        <p><strong>{itemData.fullTime}</strong></p>
                        <p>🌡️ Temperature: {itemData.temperature?.toFixed(1)} °C</p>
                        <p>💧 Humidity: {itemData.humidity?.toFixed(1)} %</p>
                        <p>🌫️ CO₂: {itemData.co2} ppm</p>
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
                  name="Temperature (°C)"
                  dot={{ r: 3 }}
                />
                <Line
                  yAxisId="left"
                  type="monotone"
                  dataKey="humidity"
                  stroke="#ff7f0e"
                  strokeWidth={2}
                  name="Humidity (%)"
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
              <h3>Latest Readings</h3>
              <p><strong>MAC:</strong> {latest.mac || 'N/A'}</p>
              {latest.name && <p><strong>Name:</strong> {latest.name}</p>}
              <p><strong>Temperature:</strong> {Math.round(latest.temp)} °C</p>
              <p><strong>Humidity:</strong> {Math.round(latest.hum)} %</p>
              <p><strong>CO₂:</strong> {latest.co2} ppm</p>
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default SensorPage;