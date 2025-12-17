# Oxygen Sensor Project

A full-stack IoT application for monitoring CO₂, temperature, and humidity using ESP32 sensors, .NET backend API, and React frontend.

## 🏗️ Architecture

- **ESP32**: Arduino-based sensor device collecting CO₂, temperature, and humidity data
- **Backend**: .NET 9.0 Web API with PostgreSQL database
- **Frontend**: React application with real-time data visualization
- **Deployment**: 
  - Frontend: Cloudflare Pages
  - Backend: Docker container with Cloudflare Tunnel

## 📁 Project Structure

```
Oxygen-Sensor-Project/
├── ESP32Storage/          # ESP32 Arduino code
├── backend/               # .NET 9.0 Web API
│   ├── Controllers/       # API controllers
│   ├── Data/              # Database context
│   ├── Models/            # Data models
│   ├── Services/          # Business logic
│   ├── Dockerfile         # Docker configuration
│   └── entrypoint.sh      # Container entrypoint
├── frontend/              # React application
│   └── src/
│       ├── components/    # React components
│       └── pages/         # Page components
├── .github/
│   └── workflows/
│       └── deploy.yml     # CI/CD pipeline
└── docker-compose.yml     # Local development setup
```

## 🚀 Quick Start

### Prerequisites

- Node.js 18+ and npm
- .NET 9.0 SDK
- Docker and Docker Compose (for containerized setup)
- PostgreSQL database

### Local Development

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Oxygen-Sensor-Project
   ```

2. **Backend Setup**
   ```bash
   cd backend
   # Update appsettings.json with your PostgreSQL connection string
   dotnet restore
   dotnet run
   ```

3. **Frontend Setup**
   ```bash
   cd frontend
   npm install
   # Create .env.local with: REACT_APP_API_URL=http://localhost:5245
   npm start
   ```

4. **Using Docker Compose** (includes PostgreSQL)
   ```bash
   docker-compose up -d
   ```

## 🔧 Configuration

### Backend Environment Variables

- `ConnectionStrings__PostgresConnection`: PostgreSQL connection string
- `AllowedOrigins__0`, `AllowedOrigins__1`, etc.: CORS allowed origins
- `CLOUDFLARE_TUNNEL_TOKEN`: Cloudflare Tunnel token (optional, for production)
- `ASPNETCORE_ENVIRONMENT`: `Development` or `Production`

### Frontend Environment Variables

- `REACT_APP_API_URL`: Backend API URL (e.g., `http://localhost:5245` or `https://api.yourdomain.com`)

## 📦 CI/CD Pipeline

This project uses GitHub Actions for automated deployment:

- **Frontend**: Automatically builds and deploys to Cloudflare Pages on push to `main` or `dev` branches
- **Backend**: Builds Docker image and pushes to GitHub Container Registry

### Required GitHub Secrets

See [DEPLOYMENT.md](./DEPLOYMENT.md) for detailed setup instructions.

Required secrets:
- `CLOUDFLARE_API_TOKEN`
- `CLOUDFLARE_ACCOUNT_ID`
- `CLOUDFLARE_TUNNEL_TOKEN`
- `POSTGRES_CONNECTION_STRING`
- `REACT_APP_API_URL`
- `FRONTEND_URL` (optional, defaults to Cloudflare Pages URL)

## 🐳 Docker

### Build Backend Image

```bash
cd backend
docker build -t oxygen-sensor-backend .
```

### Run Backend Container

```bash
docker run -d \
  --name oxygen-sensor-backend \
  -p 8080:8080 \
  -e ConnectionStrings__PostgresConnection="Host=localhost;Port=5432;Database=postgres;Username=user;Password=pass" \
  -e CLOUDFLARE_TUNNEL_TOKEN="your-token" \
  oxygen-sensor-backend
```

## 📡 API Endpoints

- `POST /sensor/receive` - Receive sensor data from ESP32
- `GET /sensor/devices` - Get list of all devices
- `GET /sensor/latest` - Get latest sensor data
- `GET /sensor/all` - Get all sensor data (last 50 records)
- `GET /sensor/history?period={live|hour|day|week}&mac={mac}` - Get historical data
- `GET /sensor/export/csv?period={period}&mac={mac}` - Export data as CSV

## 🔒 Security

- Never commit secrets or credentials
- Use environment variables for sensitive data
- Configure CORS properly for production
- Use HTTPS in production
- Rotate API tokens regularly

## 📚 Documentation

- [Deployment Guide](./DEPLOYMENT.md) - Detailed deployment instructions
- [Cloudflare Tunnel Config](./cloudflare-tunnel-config.yml) - Tunnel setup template

## 🤝 Contributing

1. Create a feature branch
2. Make your changes
3. Submit a pull request

## 📝 License

[Add your license here]

## 🆘 Support

For issues or questions:
- Check GitHub Actions logs
- Review Cloudflare dashboard
- Check Docker container logs
- Review application logs
