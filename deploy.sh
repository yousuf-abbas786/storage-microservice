#!/bin/bash

# Storage Service Deployment Script
# This script helps deploy the Storage Service on a remote machine

set -e

echo "=========================================="
echo "Storage Service Deployment Script"
echo "=========================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo -e "${RED}Error: Docker is not installed. Please install Docker first.${NC}"
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker compose &> /dev/null && ! command -v docker-compose &> /dev/null; then
    echo -e "${RED}Error: Docker Compose is not installed. Please install Docker Compose first.${NC}"
    exit 1
fi

echo -e "${GREEN}✓ Docker and Docker Compose are installed${NC}"

# Check if .env file exists
if [ ! -f .env ]; then
    echo -e "${YELLOW}Warning: .env file not found. Using default values.${NC}"
    echo "For production, create a .env file with secure passwords."
else
    echo -e "${GREEN}✓ .env file found${NC}"
fi

# Ask for deployment type
echo ""
echo "Select deployment type:"
echo "1) Development (default)"
echo "2) Production"
read -p "Enter choice [1-2] (default: 1): " deploy_type
deploy_type=${deploy_type:-1}

# Build and start services
echo ""
echo "Building and starting services..."
echo ""

if [ "$deploy_type" = "2" ]; then
    echo -e "${YELLOW}Starting in PRODUCTION mode...${NC}"
    docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
else
    echo -e "${YELLOW}Starting in DEVELOPMENT mode...${NC}"
    docker compose up -d --build
fi

# Wait for services to be ready
echo ""
echo "Waiting for services to be ready..."
sleep 10

# Check container status
echo ""
echo "Container Status:"
docker compose ps

# Check if containers are running
if docker compose ps | grep -q "Up"; then
    echo ""
    echo -e "${GREEN}=========================================="
    echo "✓ Deployment successful!"
    echo "==========================================${NC}"
    echo ""
    echo "Services are running:"
    echo "  - API: http://localhost:8080"
    echo "  - Swagger: http://localhost:8080/swagger"
    echo ""
    echo "Useful commands:"
    echo "  - View logs: docker compose logs -f"
    echo "  - Stop services: docker compose down"
    echo "  - Restart: docker compose restart"
    echo ""
else
    echo ""
    echo -e "${RED}=========================================="
    echo "✗ Some containers failed to start"
    echo "==========================================${NC}"
    echo ""
    echo "Check logs with: docker compose logs"
    exit 1
fi

