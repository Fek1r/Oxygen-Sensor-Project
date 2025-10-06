// src/components/Header.jsx
import React from "react";
import { useNavigate } from "react-router-dom";
import "./Header.css";

const Header = ({ onLogout }) => {
  const navigate = useNavigate();

  const handleLogoutClick = () => {
    onLogout();
    navigate("/login");
  };

  return (
    <aside className="header">
      <h2
        className="logo"
        onClick={() => navigate("/")}
        style={{ cursor: "pointer" }}
      >
        O₂ Monitor
      </h2>

      <nav>
        <ul>
          <li onClick={() => navigate("/")} style={{ cursor: "pointer" }}>
            Главная
          </li>
          <li onClick={handleLogoutClick} style={{ cursor: "pointer", color: "#e74c3c" }}>
            Выход
          </li>
        </ul>
      </nav>
    </aside>
  );
};

export default Header;