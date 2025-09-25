import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Header from "./components/Header";
import HomePage from "./pages/HomePage";
import SensorPage from "./pages/SensorPage";
import "./App.css";

function App() {
  return (
    <Router>
      <div className="app">
        <Header />
        <div className="main-content">
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/sensor" element={<SensorPage />} />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;
