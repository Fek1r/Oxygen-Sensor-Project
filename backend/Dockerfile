# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["SensorApi.csproj", "./"]
RUN dotnet restore "SensorApi.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "SensorApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SensorApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install Cloudflare tunnel (cloudflared)
RUN apt-get update && \
    apt-get install -y wget && \
    wget -q https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64.deb && \
    dpkg -i cloudflared-linux-amd64.deb && \
    rm cloudflared-linux-amd64.deb && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy entrypoint script
COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]
