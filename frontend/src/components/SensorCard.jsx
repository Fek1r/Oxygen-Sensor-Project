import React from "react";
import "./SensorCard.css";

const SensorCard = ({ title, description, onSelect }) => {
  return (
    <div className="sensor-card">
      <div className="sensor-img">
        <span role="img" aria-label="sensor">🩺</span>
      </div>
      <h3>{title}</h3>
      <p>{description}</p>
      <button onClick={onSelect}>Выбрать</button>
    </div>
  );
};

export default SensorCard;
