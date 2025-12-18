import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import SensorCard from "../components/SensorCard";
import "./HomePage.css";

const HomePage = () => {
  const navigate = useNavigate();
  const [devices, setDevices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const baseUrl = process.env.REACT_APP_API_URL;

  useEffect(() => {
    const fetchDevices = async () => {
      try {
        const response = await fetch(`${baseUrl}/sensor/devices`);
        const text = await response.text();

        let data;
        try {
          data = JSON.parse(text);
        } catch (err) {
          console.error("JSON parsing error:", text);
          setError("Server did not return valid JSON");
          setLoading(false);
          return;
        }

        setDevices(data);
        setError(null);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchDevices();

    // Refresh every 10 seconds
    const interval = setInterval(fetchDevices, 10000);
    return () => clearInterval(interval);
  }, [baseUrl]);

  if (loading) {
    return (
      <div className="homepage">
        <h1>Loading devices...</h1>
      </div>
    );
  }

  if (error) {
    return (
      <div className="homepage">
        <h1>Error</h1>
        <p style={{ color: "red" }}>{error}</p>
      </div>
    );
  }

  const onlineDevices = devices.filter(d => d.isOnline);
  const offlineDevices = devices.filter(d => !d.isOnline);

  return (
    <div className="homepage">
      <h1>Welcome!</h1>
      <p>Select a sensor to start monitoring.</p>

      {devices.length === 0 ? (
        <p className="no-devices">
          No devices found. Please ensure your ESP32 is sending data to the server.
        </p>
      ) : (
        <>
          {onlineDevices.length > 0 && (
            <>
              <h2 className="section-title">
                🟢 Active Devices ({onlineDevices.length})
              </h2>
              <div className="sensor-list">
                {onlineDevices.map((device) => (
                  <SensorCard
                    key={device.mac}
                    title={device.name || `Device ${device.mac}`}
                    description={`MAC: ${device.mac}`}
                    extraInfo={`CO₂: ${device.co2} ppm | Temp: ${device.temp.toFixed(1)}°C`}
                    isOnline={true}
                    onSelect={() => navigate(`/sensor/${device.mac}`)}
                  />
                ))}
              </div>
            </>
          )}

          {offlineDevices.length > 0 && (
            <>
              <h2 className="section-title offline-section">
                🔴 Inactive Devices ({offlineDevices.length})
              </h2>
              <div className="sensor-list">
                {offlineDevices.map((device) => (
                  <SensorCard
                    key={device.mac}
                    title={device.name || `Device ${device.mac}`}
                    description={`MAC: ${device.mac}`}
                    extraInfo={`Last data: CO₂: ${device.co2} ppm`}
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