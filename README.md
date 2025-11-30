# Storage Service

A microservice for centralized file storage with support for local and cloud storage providers.

## Architecture

Clean Architecture with clear separation of concerns:
- **API**: Presentation layer with minimal endpoints
- **Application**: Business logic and use cases
- **Domain**: Core entities and business rules
- **Infrastructure**: Data access, storage providers, external services

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Docker Desktop (for containerized deployment)
- SQL Server (or use Docker container)

### Local Development

1. Update connection string in `appsettings.Development.json`
2. Create initial migration:
   ```bash
   dotnet ef migrations add InitialCreate --project StorageService.Infrastructure --startup-project StorageService.API
   ```
3. Run the application:
   ```bash
   dotnet run --project StorageService.API
   ```

### Docker Deployment

#### Local Development

Build and run with Docker Compose:
```bash
docker compose up --build
```

The service will:
- Start SQL Server container
- Run database migrations automatically
- Expose API on http://localhost:8080
- Swagger UI available at http://localhost:8080/swagger

#### Production Deployment on Separate Machine

For deploying on a separate machine, see **[DEPLOYMENT.md](DEPLOYMENT.md)** for detailed instructions.

**Quick Start:**
```bash
# 1. Transfer project files to target machine
# 2. On target machine, navigate to project directory
cd StorageService

# 3. Build and start services
docker compose up -d --build

# 4. Check status
docker compose ps

# 5. View logs
docker compose logs -f

# 6. Access API
# http://<target-machine-ip>:8080/swagger
```

**For production use:**
```bash
# Create .env file with secure passwords
# Then run with production config
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

## API Endpoints

- `POST /api/Files` - Upload a file
- `GET /api/Files/{id}` - Download a file
- `DELETE /api/Files/{id}` - Delete a file

## Configuration

- `ConnectionStrings:DefaultConnection` - SQL Server connection string
- `Storage:RootPath` - Local file storage root path

