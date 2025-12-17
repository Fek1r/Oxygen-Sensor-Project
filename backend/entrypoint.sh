#!/bin/bash
set -e

# Start Cloudflare tunnel in background if token is provided
if [ -n "$CLOUDFLARE_TUNNEL_TOKEN" ]; then
  echo "Starting Cloudflare tunnel with token..."
  # Export token for cloudflared to use
  export TUNNEL_TOKEN=$CLOUDFLARE_TUNNEL_TOKEN
  # Run the tunnel - cloudflared will use TUNNEL_TOKEN from environment
  cloudflared tunnel run &
  TUNNEL_PID=$!
  echo "Cloudflare tunnel started with PID: $TUNNEL_PID"
  
  # Wait a bit for tunnel to initialize
  sleep 3
fi

# Start the .NET application
echo "Starting .NET application..."
exec dotnet SensorApi.dll
