import React from "react";
import "./SensorCard.css";

const SensorCard = ({ title, description, extraInfo, isOnline, onSelect }) => {
  return (
    <div 
      className={`sensor-card ${!isOnline ? 'offline' : ''}`} 
      onClick={isOnline ? onSelect : null}
      style={{ cursor: isOnline ? 'pointer' : 'not-allowed' }}
    >
      <div className="card-header">
        <h3>{title}</h3>
        <span className={`status-badge ${isOnline ? 'online' : 'offline'}`}>
          {isOnline ? '🟢 Online' : '🔴 Offline'}
        </span>
      </div>
      <p className="description">{description}</p>
      {extraInfo && <p className="extra-info">{extraInfo}</p>}
      {isOnline ? (
        <button className="connect-button">Conect</button>
      ) : (
        <button className="connect-button disabled" disabled>
          Dissable
        </button>
      )}
    </div>
  );
};

export default SensorCard;