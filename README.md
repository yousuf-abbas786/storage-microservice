# Storage Service

A microservice for centralized file storage with support for local and cloud storage providers. This service acts as a storage hub for all other microservices in the system, providing a unified interface for file operations regardless of the underlying storage backend.

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

## Design Documentation

### 1. Functional and Non-Functional Requirements

#### Functional Requirements

**Core Functionality:**
- File upload: Accept files of any extension via HTTP POST with multipart/form-data
- File download: Retrieve files by unique identifier via HTTP GET
- File deletion: Soft delete files (mark as deleted, retain metadata for audit)
- File listing: Paginated list of all files with metadata
- File search: Search files by name with pagination support
- Metadata storage: Store file metadata (name, size, content type, owner, tenant) in SQL Server
- Multi-tenant support: Support tenant and owner isolation for file access
- Storage abstraction: Support multiple storage backends (currently local filesystem, extensible to AWS S3, Azure Blob Storage)

**Authentication & Authorization:**
- JWT Bearer authentication: Token-based authentication for all file operations
- User registration: Self-service user registration with tenant scoping
- User login: JWT token generation upon successful authentication
- Tenant-based access: Users are scoped to specific tenants

**Extended Features:**
- Container organization: Files organized into containers (folders/buckets)
- Custom metadata: JSON field for extensible metadata storage
- Audit trail: Created and deleted timestamps for all files
- API versioning: URL-based, query string, header, and media type versioning support

#### Non-Functional Requirements

**Performance:**
- Support concurrent file uploads/downloads
- Stream large files without buffering in memory
- Stateless design allows horizontal scaling
- Efficient pagination for large file lists
- Database indexes for fast file name searches

**Reliability:**
- Transactional consistency between metadata and storage
- Global exception handler with consistent error responses
- Full cancellation token support throughout the application
- Health checks for database and storage connectivity

**Security:**
- Input validation: File size limits, content type validation
- JWT authentication: All file operations require valid JWT token
- Password hashing: SHA256 password hashing for user credentials
- Access control: Tenant and owner-based isolation
- SQL injection protection: Parameterized queries via EF Core

**Observability:**
- Structured logging: Serilog with file and console sinks
- Log rotation: Daily log file rotation with 30-day retention
- Exception logging: All exceptions logged with full context

**Maintainability:**
- Clean Architecture: Clear layer separation and dependency direction
- Options Pattern: Strongly-typed configuration throughout
- Dependency Injection: Clean, organized service registration
- Extensibility: Easy to add new storage providers
- Testability: Interfaces enable unit and integration testing

### 2. High-Level and Low-Level Design

#### High-Level Design

The microservice follows Clean Architecture principles with four distinct layers:

```
┌─────────────────────────────────────────────────────────────┐
│              Other Microservices / Applications              │
│                    (Client Services)                         │
└───────────────────────────┬─────────────────────────────────┘
                            │ HTTP/REST API
                            │ JWT Bearer Token
                            │
┌───────────────────────────▼─────────────────────────────────┐
│                      API Layer                               │
│  - Endpoints (Files, Auth)                                  │
│  - Middleware (Exception Handling, Swagger)                  │
│  - API Versioning Support                                    │
└───────────────────────────┬─────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────┐
│                   Application Layer                          │
│  - FileService (Business Logic)                              │
│  - AuthService (Authentication)                              │
│  - DTOs (Data Transfer Objects)                              │
│  - Specifications (Query Logic)                               │
└───────────────┬───────────────────────────┬────────────────┘
                │                           │
┌───────────────▼──────────────┐  ┌────────▼─────────────────┐
│      Domain Layer            │  │   Infrastructure Layer    │
│  - StoredFile Entity         │  │  - StorageDbContext      │
│  - User Entity               │  │  - IFileStorageProvider  │
│  - Business Rules            │  │  - Repositories           │
└──────────────────────────────┘  └────────┬─────────────────┘
                            ┌───────────────┴───────────────┐
                            │                               │
                  ┌─────────▼─────────┐         ┌─────────▼─────────┐
                  │   SQL Server       │         │  File Storage      │
                  │   (Metadata)      │         │  (Local/Cloud)     │
                  └────────────────────┘         └────────────────────┘
```

#### Low-Level Design

**API Layer (`StorageService.API`):**
- Handles HTTP request/response with versioning
- Validates JWT tokens
- Maps HTTP requests to application services
- Formats responses using consistent APIResult structure
- No business logic - pure translation layer

**Application Layer (`StorageService.Application`):**
- `FileService`: Orchestrates file operations, coordinates between storage provider and database
- `AuthService`: Handles authentication and JWT token generation
- Uses Specification Pattern for query logic
- DTOs for API contracts

**Domain Layer (`StorageService.Domain`):**
- `StoredFile`: Core entity representing file metadata
- `User`: Authentication entity with tenant scoping
- Pure domain model with no dependencies

**Infrastructure Layer (`StorageService.Infrastructure`):**
- `StorageDbContext`: EF Core DbContext for SQL Server
- `LocalFileStorageProvider`: Local filesystem implementation
- `Repository<T>`: Generic repository with Specification Pattern support
- Future: AWS S3, Azure Blob Storage implementations

**Data Flow Example - File Upload:**
1. Client sends POST request with file to `/api/v1/Files`
2. Endpoint validates JWT token
3. Endpoint extracts file stream from multipart/form-data
4. FileService.UploadAsync:
   - Calls IFileStorageProvider.StoreAsync → writes file to storage
   - Creates StoredFile entity with metadata
   - Saves to database via Repository
   - Returns UploadFileResponse DTO
5. Endpoint returns 201 Created with file ID

### 3. Storage Type and File Save/Retrieval Mechanism

#### Current Implementation: Local File Storage

**Storage Type:**
- **Currently**: Local filesystem storage (suitable for development and small-scale deployments)
- **Extensible**: Architecture supports easy migration to cloud storage providers (AWS S3, Azure Blob Storage)

**Why Local Storage for Now:**
- Simple setup with no external dependencies
- Fast for development and testing
- Easy to debug and inspect files
- No additional costs during development
- Can be easily migrated to cloud storage later without code changes

**File Storage Structure:**
```
storage/
  └── {container}/
      └── {yyyy/MM/dd}/
          └── {guid}.{extension}
```

Example: `storage/main/2025/11/30/a1b2c3d4-e5f6-7890-abcd-ef1234567890.pdf`

**How Files are Saved:**
1. File received via HTTP POST with multipart/form-data
2. File stream is passed to `IFileStorageProvider.StoreAsync()`
3. Storage provider generates unique file key: `{yyyy/MM/dd}/{guid}.{extension}`
4. File is written to: `{container}/{fileKey}`
5. Metadata (filename, size, content type, owner, tenant) is saved to SQL Server
6. Returns file ID (GUID) to client

**How Files are Retrieved:**
1. Client requests file via GET `/api/v1/Files/{id}`
2. System queries SQL Server for StoredFile by ID
3. Retrieves storage key and container from metadata
4. Calls `IFileStorageProvider.GetAsync(container, storageKey)`
5. Returns file stream directly to HTTP response
6. Client receives file with original filename and content type

**Storage Abstraction:**
The service uses the `IFileStorageProvider` interface, which allows switching storage backends without changing application code:
- **Current**: `LocalFileStorageProvider` - stores files on local filesystem
- **Future**: `S3FileStorageProvider` - stores files in AWS S3
- **Future**: `AzureBlobStorageProvider` - stores files in Azure Blob Storage

Storage provider is selected via configuration (`appsettings.json`):
```json
{
  "Storage": {
    "Provider": "Local",  // Can be changed to "S3" or "AzureBlob" in future
    "RootPath": "./storage"
  }
}
```

### 4. Microservice Communication

#### Communication Protocol

**Synchronous HTTP/REST:**
- Other microservices communicate with Storage Service via HTTP/REST API
- Protocol: HTTP/HTTPS over standard ports
- Format: JSON for requests/responses, multipart/form-data for file uploads

#### Authentication

**JWT Bearer Token:**
1. Client microservice calls `/api/v1/Auth/login` with credentials
2. Storage Service validates credentials and returns JWT token
3. Client includes token in `Authorization: Bearer <token>` header for all subsequent requests
4. Storage Service validates token on each request

**Example Request:**
```http
POST /api/v1/Files
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: multipart/form-data

file: [binary data]
ownerId: user123
tenantId: TenantABC
```

#### API Endpoints for Microservice Integration

**Authentication:**
- `POST /api/v1/Auth/register` - Register new user/tenant
- `POST /api/v1/Auth/login` - Get JWT token

**File Operations:**
- `POST /api/v1/Files` - Upload file (multipart/form-data)
- `GET /api/v1/Files/{id}` - Download file (returns file stream)
- `GET /api/v1/Files` - List files (paginated, query params: pageNumber, pageSize)
- `GET /api/v1/Files/search?fileName=...` - Search files by name
- `DELETE /api/v1/Files/{id}` - Soft delete file

#### API Versioning

The service supports multiple versioning strategies:
- **URL Segment**: `/api/v1/Files`, `/api/v2/Files`
- **Query String**: `/api/Files?api-version=1.0`
- **Header**: `X-Version: 1.0`
- **Media Type**: `application/json;ver=1.0`

#### Response Format

All responses follow consistent `APIResult` structure:
```json
{
  "statusCode": 200,
  "message": "Ok",
  "data": { ... }
}
```

#### Error Handling

- Consistent error format across all endpoints
- HTTP status codes: 200 (OK), 201 (Created), 400 (Bad Request), 401 (Unauthorized), 404 (Not Found), 500 (Internal Server Error)
- Error details included in response body

#### Service Discovery

In containerized environments:
- **Docker Compose**: Services discoverable by service name (`storage-api`)
- **Kubernetes**: Service names resolve via DNS
- **Base URL**: `http://storage-api:8080/api/v1` (internal) or `http://<host-ip>:8080/api/v1` (external)

#### Integration Example

**Step 1: Authenticate**
```http
POST http://storage-service:8080/api/v1/Auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "123",
  "tenantId": "yousuf"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-12-01T14:00:00Z"
}
```

**Step 2: Upload File**
```http
POST http://storage-service:8080/api/v1/Files
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: multipart/form-data

file: [binary]
ownerId: user123
tenantId: TenantABC
```

**Response:**
```json
{
  "statusCode": 201,
  "message": "Ok",
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "fileName": "document.pdf",
    "contentType": "application/pdf",
    "size": 1024,
    "container": "main",
    "storageKey": "2025/11/30/a1b2c3d4-e5f6-7890-abcd-ef1234567890.pdf",
    "createdAt": "2025-11-30T14:00:00Z"
  }
}
```

**Step 3: Download File**
```http
GET http://storage-service:8080/api/v1/Files/a1b2c3d4-e5f6-7890-abcd-ef1234567890
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response:** File binary stream with appropriate Content-Type and Content-Disposition headers.

#### Future Enhancements

- **Event-Driven**: Publish file events (uploaded, deleted) to message queue
- **Webhooks**: Notify other services on file operations
- **Batch Operations**: Bulk upload/download endpoints
- **Async Operations**: Long-running operations with status polling

For detailed architecture information, see [ARCHITECTURE.md](ARCHITECTURE.md).
