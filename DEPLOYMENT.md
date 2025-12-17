# Deployment Guide

This guide explains how to deploy the Oxygen Sensor Project using GitHub Actions, Cloudflare Pages, and Cloudflare Tunnel.

## Architecture

- **Frontend**: React app deployed to Cloudflare Pages
- **Backend**: .NET 9.0 API containerized with Docker and exposed via Cloudflare Tunnel

## Prerequisites

1. GitHub repository with Actions enabled
2. Cloudflare account
3. PostgreSQL database (can be Cloudflare D1, Supabase, or any PostgreSQL provider)
4. Docker (for local testing)

## Setup Instructions

### 1. Cloudflare Pages Setup

1. Go to [Cloudflare Dashboard](https://dash.cloudflare.com/)
2. Navigate to **Workers & Pages** → **Pages**
3. Create a new project (or connect to GitHub)
4. Note your Cloudflare Account ID

### 2. Cloudflare Tunnel Setup

#### Option A: Using Tunnel Token (Recommended for Docker)

1. Go to [Cloudflare Zero Trust Dashboard](https://one.dash.cloudflare.com/)
2. Navigate to **Networks** → **Tunnels**
3. Click **Create a tunnel**
4. Select **Cloudflared** as the connector
5. Give it a name (e.g., `oxygen-sensor-backend`)
6. Copy the **Tunnel Token**

#### Option B: Using Named Tunnel

1. Install cloudflared on your server
2. Run: `cloudflared tunnel login`
3. Run: `cloudflared tunnel create oxygen-sensor-backend`
4. Create a config file and route traffic

### 3. GitHub Secrets Configuration

Add the following secrets to your GitHub repository (**Settings** → **Secrets and variables** → **Actions**):

#### Required Secrets:

- `CLOUDFLARE_API_TOKEN`: Your Cloudflare API token with Pages and Tunnel permissions
- `CLOUDFLARE_ACCOUNT_ID`: Your Cloudflare Account ID
- `CLOUDFLARE_TUNNEL_TOKEN`: Your Cloudflare Tunnel token (from step 2)
- `POSTGRES_CONNECTION_STRING`: PostgreSQL connection string (e.g., `Host=hostname;Port=5432;Database=dbname;Username=user;Password=pass`)
- `REACT_APP_API_URL`: Your backend API URL (e.g., `https://api.yourdomain.com`)

#### How to get Cloudflare API Token:

1. Go to [Cloudflare API Tokens](https://dash.cloudflare.com/profile/api-tokens)
2. Click **Create Token**
3. Use **Edit Cloudflare Workers** template or create custom token with:
   - **Account** → **Cloudflare Pages** → **Edit**
   - **Account** → **Cloudflare Tunnel** → **Edit**
4. Copy the token

### 4. Backend Configuration

Update `backend/appsettings.json` or use environment variables:

```json
{
  "ConnectionStrings": {
    "PostgresConnection": "Host=...;Port=5432;Database=...;Username=...;Password=..."
  },
  "AllowedOrigins": [
    "http://localhost:3000",
    "https://your-frontend-domain.pages.dev"
  ]
}
```

### 5. Frontend Configuration

The frontend uses `REACT_APP_API_URL` environment variable. This is set during the build process in the GitHub Actions workflow.

For local development, create `frontend/.env.local`:

```
REACT_APP_API_URL=http://localhost:5245
```

## Deployment Flow

### Automatic Deployment (via GitHub Actions)

1. Push to `main` or `dev` branch
2. GitHub Actions will:
   - Build and deploy frontend to Cloudflare Pages
   - Build Docker image and push to GitHub Container Registry
   - Provide deployment instructions

### Manual Backend Deployment

If you need to deploy the backend manually:

```bash
# Pull the latest image
docker pull ghcr.io/YOUR_USERNAME/Oxygen-Sensor-Project/backend:latest

# Run the container
docker run -d \
  --name oxygen-sensor-backend \
  --restart unless-stopped \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__PostgresConnection="YOUR_CONNECTION_STRING" \
  -e CLOUDFLARE_TUNNEL_TOKEN="YOUR_TUNNEL_TOKEN" \
  -e AllowedOrigins__0="https://your-frontend-domain.pages.dev" \
  ghcr.io/YOUR_USERNAME/Oxygen-Sensor-Project/backend:latest
```

## Testing Locally

### Backend

```bash
cd backend
docker build -t oxygen-sensor-backend .
docker run -p 8080:8080 \
  -e ConnectionStrings__PostgresConnection="Host=localhost;Port=5432;Database=postgres;Username=user;Password=pass" \
  oxygen-sensor-backend
```

### Frontend

```bash
cd frontend
npm install
REACT_APP_API_URL=http://localhost:8080 npm start
```

## Troubleshooting

### Frontend not connecting to backend

1. Check CORS configuration in `backend/Program.cs`
2. Verify `REACT_APP_API_URL` is set correctly
3. Check browser console for CORS errors

### Cloudflare Tunnel not working

1. Verify `CLOUDFLARE_TUNNEL_TOKEN` is correct
2. Check tunnel status in Cloudflare Zero Trust dashboard
3. Review container logs: `docker logs oxygen-sensor-backend`

### Database connection issues

1. Verify PostgreSQL connection string format
2. Check database is accessible from your deployment location
3. Ensure database exists and migrations are applied

## Environment Variables Reference

### Backend

- `ASPNETCORE_ENVIRONMENT`: `Development` | `Production`
- `ASPNETCORE_URLS`: Server URLs (default: `http://+:8080`)
- `ConnectionStrings__PostgresConnection`: PostgreSQL connection string
- `AllowedOrigins__0`, `AllowedOrigins__1`, etc.: CORS allowed origins
- `CLOUDFLARE_TUNNEL_TOKEN`: Cloudflare Tunnel token (optional)

### Frontend

- `REACT_APP_API_URL`: Backend API URL

## Security Notes

- Never commit secrets to the repository
- Use GitHub Secrets for sensitive data
- Rotate API tokens regularly
- Use strong database passwords
- Enable HTTPS for production
- Configure proper CORS origins

## Support

For issues or questions, please check:
- GitHub Actions logs
- Cloudflare dashboard
- Docker container logs
- Application logs
