# Storage Service Deployment Script for Windows
# This script helps deploy the Storage Service on a remote Windows machine

param(
    [string]$DeployType = "Development"
)

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Storage Service Deployment Script" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Check if Docker is installed
try {
    $dockerVersion = docker --version
    Write-Host "✓ Docker is installed: $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ Error: Docker is not installed. Please install Docker Desktop first." -ForegroundColor Red
    exit 1
}

# Check if Docker Compose is available
try {
    docker compose version | Out-Null
    Write-Host "✓ Docker Compose is available" -ForegroundColor Green
} catch {
    Write-Host "✗ Error: Docker Compose is not available." -ForegroundColor Red
    exit 1
}

# Check if .env file exists
if (Test-Path .env) {
    Write-Host "✓ .env file found" -ForegroundColor Green
} else {
    Write-Host "⚠ Warning: .env file not found. Using default values." -ForegroundColor Yellow
    Write-Host "  For production, create a .env file with secure passwords." -ForegroundColor Yellow
}

Write-Host ""

# Ask for deployment type if not specified
if (-not $DeployType) {
    Write-Host "Select deployment type:"
    Write-Host "1) Development (default)"
    Write-Host "2) Production"
    $choice = Read-Host "Enter choice [1-2] (default: 1)"
    
    if ($choice -eq "2") {
        $DeployType = "Production"
    } else {
        $DeployType = "Development"
    }
}

# Build and start services
Write-Host ""
Write-Host "Building and starting services..." -ForegroundColor Yellow
Write-Host ""

if ($DeployType -eq "Production") {
    Write-Host "Starting in PRODUCTION mode..." -ForegroundColor Yellow
    docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
} else {
    Write-Host "Starting in DEVELOPMENT mode..." -ForegroundColor Yellow
    docker compose up -d --build
}

# Wait for services to be ready
Write-Host ""
Write-Host "Waiting for services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Check container status
Write-Host ""
Write-Host "Container Status:" -ForegroundColor Cyan
docker compose ps

# Check if containers are running
$containers = docker compose ps --format json | ConvertFrom-Json
$runningContainers = $containers | Where-Object { $_.State -eq "running" }

if ($runningContainers.Count -gt 0) {
    Write-Host ""
    Write-Host "==========================================" -ForegroundColor Green
    Write-Host "✓ Deployment successful!" -ForegroundColor Green
    Write-Host "==========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Services are running:" -ForegroundColor Cyan
    Write-Host "  - API: http://localhost:8080"
    Write-Host "  - Swagger: http://localhost:8080/swagger"
    Write-Host ""
    Write-Host "Useful commands:" -ForegroundColor Cyan
    Write-Host "  - View logs: docker compose logs -f"
    Write-Host "  - Stop services: docker compose down"
    Write-Host "  - Restart: docker compose restart"
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "==========================================" -ForegroundColor Red
    Write-Host "✗ Some containers failed to start" -ForegroundColor Red
    Write-Host "==========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Check logs with: docker compose logs" -ForegroundColor Yellow
    exit 1
}

