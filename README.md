# Storage Service

A microservice for centralized file storage with support for local and cloud storage providers.

## Prerequisites

- Docker Desktop installed and running
- Git (for cloning the repository)

## Quick Start

### 1. Clone the Repository

```bash
git clone <your-repository-url>
cd StorageService
```

### 2. Start the Services

```bash
docker compose up -d --build
```

This command will:
- Download required Docker images (first time only)
- Build the application
- Start SQL Server container
- Start the API container
- Run database migrations automatically
- Create default admin user

### 3. Access the API

Open your browser and navigate to:
- **Swagger UI**: http://localhost:8080/swagger
- **API Base URL**: http://localhost:8080/api/v1

### 4. Default Credentials

- **Username**: `admin`
- **Password**: `123`
- **Tenant**: `yousuf`

## Deployment on Separate Machine

### Step 1: Clone on Target Machine

```bash
git clone <your-repository-url>
cd StorageService
```

### Step 2: Start Services

```bash
docker compose up -d --build
```

### Step 3: Verify Deployment

```bash
# Check container status
docker compose ps

# View logs
docker compose logs -f
```

### Step 4: Access from Remote

1. Ensure port 8080 is open in firewall
2. Access via: `http://<target-machine-ip>:8080/swagger`

## Useful Commands

```bash
# View logs
docker compose logs -f

# Stop services
docker compose down

# Restart services
docker compose restart

# Rebuild and restart
docker compose up -d --build
```

## API Endpoints

- `POST /api/v1/Auth/register` - Register new user
- `POST /api/v1/Auth/login` - Login and get JWT token
- `POST /api/v1/Files` - Upload a file
- `GET /api/v1/Files` - List all files (paginated)
- `GET /api/v1/Files/search?fileName=...` - Search files by name
- `GET /api/v1/Files/{id}` - Download a file
- `DELETE /api/v1/Files/{id}` - Delete a file

## Troubleshooting

**Containers won't start:**
- Ensure Docker Desktop is running
- Check logs: `docker compose logs`

**Port already in use:**
- Change port in `docker-compose.yml` or stop conflicting service

**SQL Server unhealthy:**
- Wait a minute for SQL Server to fully start
- Check logs: `docker compose logs sqlserver`

For more details, see [DEPLOYMENT.md](DEPLOYMENT.md) and [TROUBLESHOOTING.md](TROUBLESHOOTING.md).
