// src/App.jsx
import React, { useState, useEffect } from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import Header from "./components/Header";
import HomePage from "./pages/HomePage";
import SensorPage from "./pages/SensorPage";
import LoginPage from "./pages/LoginPage";
import "./App.css";

function ProtectedRoute({ isAuthenticated, children }) {
  return isAuthenticated ? children : <Navigate to="/login" replace />;
}

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    // Проверяем, есть ли активная сессия в localStorage
    const authStatus = localStorage.getItem("isAuthenticated");
    if (authStatus === "true") {
      setIsAuthenticated(true);
    }
  }, []);

  const handleLogin = () => {
    setIsAuthenticated(true);
    localStorage.setItem("isAuthenticated", "true");
  };

  const handleLogout = () => {
    setIsAuthenticated(false);
    localStorage.removeItem("isAuthenticated");
  };

  return (
    <Router>
      <div className="app">
        {isAuthenticated && <Header onLogout={handleLogout} />}
        <div className="main-content">
          <Routes>
            <Route
              path="/login"
              element={
                isAuthenticated ? (
                  <Navigate to="/" replace />
                ) : (
                  <LoginPage onLogin={handleLogin} />
                )
              }
            />

            <Route
              path="/"
              element={
                <ProtectedRoute isAuthenticated={isAuthenticated}>
                  <HomePage />
                </ProtectedRoute>
              }
            />

            <Route
              path="/sensor"
              element={
                <ProtectedRoute isAuthenticated={isAuthenticated}>
                  <SensorPage />
                </ProtectedRoute>
              }
            />

            <Route
              path="/sensor/:mac"
              element={
                <ProtectedRoute isAuthenticated={isAuthenticated}>
                  <SensorPage />
                </ProtectedRoute>
              }
            />

            {/* Перенаправление всех неизвестных маршрутов */}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;
