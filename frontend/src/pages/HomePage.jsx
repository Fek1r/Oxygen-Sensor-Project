import React from "react";
import { useNavigate } from "react-router-dom";
import SensorCard from "../components/SensorCard";
import "./HomePage.css";

const HomePage = () => {
  const navigate = useNavigate();

  return (
    <div className="homepage">
      <h1>Добро пожаловать!</h1>
      <p>Здесь вы можете выбрать и подключить датчик кислорода.</p>
      
      <div className="sensor-list">
        <SensorCard 
          title="Датчик O₂ v1" 
          description="Простой и надежный." 
          onSelect={() => navigate("/sensor")} 
        />
      </div>
    </div>
  );
};

export default HomePage;
