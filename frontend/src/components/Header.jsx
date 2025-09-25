import React from "react";
import "./Header.css";

const Header = () => {
  return (
    <aside className="header">
      <h2 className="logo">O₂ Monitor</h2>
      <nav>
        <ul>
          <li>Главная</li>
          <li>Датчики</li>
          <li>Настройки</li>
        </ul>
      </nav>
    </aside>
  );
};

export default Header;
