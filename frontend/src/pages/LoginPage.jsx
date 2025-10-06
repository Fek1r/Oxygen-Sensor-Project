import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./LoginPage.css";

const LoginPage = ({ onLogin }) => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const handleSubmit = (e) => {
    e.preventDefault();

    if (username === "Admin" && password === "12345678") {
      setError("");
      onLogin();
      navigate("/");
    } else {
      setError("Неверный логин или пароль");
    }
  };

  return (
    <div className="login-page">
      <div className="login-box">
        <h1>Вход в систему</h1>
        <form onSubmit={handleSubmit}>
          <label htmlFor="username">Логин</label>
          <input
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Введите логин"
            required
          />

          <label htmlFor="password">Пароль</label>
          <input
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Введите пароль"
            required
          />

          {error && <p className="error">{error}</p>}

          <button type="submit" className="login-btn">Войти</button>
        </form>
      </div>
    </div>
  );
};

export default LoginPage;
