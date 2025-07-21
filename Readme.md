# Product Management Application

A complete full-stack product management system built with .NET 8, Blazor Server, FastEndpoints, PostgreSQL, and .NET Aspire orchestration.

## Overview

This application demonstrates modern .NET development practices using:
- **Frontend**: Blazor Server with Bootstrap 5
- **Backend**: ASP.NET Core 8 with FastEndpoints
- **Database**: PostgreSQL 15
- **Orchestration**: .NET Aspire for service discovery and container management
- **Architecture**: Clean separation of concerns with service-to-service communication

## Prerequisites

### Required Software
- **.NET 8.0 SDK** or later ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Docker Desktop** ([Download](https://www.docker.com/products/docker-desktop/))
- **PostgreSQL** (local installation OR use Docker container)

### Optional Tools
- **Visual Studio 2022** (recommended) or **Visual Studio Code**
- **Git** for version control
- **curl** for API testing
- **jq** for JSON formatting (optional)

## Project Structure

```
ProductManagementTestApp/
├── Aspire/
│   └── ProductManagementApp.Aspire/          # Main orchestration project
│       ├── Program.cs                        # Service definitions and configuration
│       ├── README.md                         # This file
│       └── Properties/launchSettings.json    # Launch configuration
├── src/
│   ├── ProductAPI/                           # Backend Web API
│   │   ├── Data/                            # Repository and data access
│   │   ├── Endpoints/                       # FastEndpoints definitions
│   │   ├── Models/                          # Data models
│   │   ├── Program.cs                       # API configuration
│   │   ├── StartStandalone.ps1              # Standalone testing script
│   │   ├── TestAPI.cmd                      # Automated test suite
│   │   ├── QUICK_START.md                   # API-specific documentation
│   │   └── TESTING_GUIDE.md                 # Comprehensive testing guide
│   └── Product.Blazor/                      # Frontend Blazor Server App
│       ├── Components/                      # UI components and pages
│       ├── Services/                        # API communication services
│       ├── Models/                          # UI models
│       └── Program.cs                       # Blazor configuration
└── src/Tests/                               # Integration tests
```

## Dependencies

### NuGet Packages
The application uses the following key dependencies:

**Aspire Orchestration:**
- `Aspire.Hosting.AppHost` - Main orchestration framework
- `Aspire.Hosting.PostgreSQL` - PostgreSQL integration

**Backend API:**
- `FastEndpoints` - Minimal API framework
- `Aspire.Npgsql` - PostgreSQL with Aspire integration
- `Dapper` - Data access micro-ORM
- `Microsoft.Extensions.ServiceDiscovery` - Service discovery

**Frontend Blazor:**
- `Microsoft.AspNetCore.Components.QuickGrid` - Data grids
- `Microsoft.Extensions.ServiceDiscovery` - Service discovery
- `Microsoft.Extensions.Http.Resilience` - HTTP resilience patterns

## Setup Instructions

### Step 1: Clone and Navigate
```bash
git clone <repository-url>
cd ProductManagementTestApp
```

### Step 2: Verify Prerequisites
```bash
# Verify .NET 8.0 SDK
dotnet --version

# Verify Docker is running
docker --version
docker ps
```

### Step 3: Database Setup

Choose one of the following options:

**Option A: Docker PostgreSQL (Recommended)**
```bash
docker run --name postgres-productmgmt \
  -e POSTGRES_DB=productdb \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=Password123 \
  -p 5432:5432 \
  -d postgres:15-alpine
```

**Option B: Local PostgreSQL Installation**
- Install PostgreSQL locally
- Create database: `productdb`
- Update connection string in `appsettings.json` if needed

### Step 4: Build Solution
```bash
# From repository root
dotnet restore
dotnet build
```

## Running the Application

### Method 1: Using Visual Studio (Recommended)
1. Open `ProductManagementTestApp.sln` in Visual Studio 2022
2. Set `ProductManagementApp.Aspire` as the startup project
3. Press F5 or click "Start Debugging"

### Method 2: Command Line
```bash
# Navigate to Aspire project
cd Aspire/ProductManagementApp.Aspire

# Run the orchestration host
dotnet run
```

### Application URLs
Once running, the following endpoints will be available:

| Service | URL | Description |
|---------|-----|-------------|
| **Aspire Dashboard** | https://localhost:17152 | Main monitoring and management dashboard |
| **Blazor Frontend** | https://localhost:5002 | Product management web interface |
| **ProductAPI** | https://localhost:7071 | Backend REST API |
| **API Health Check** | https://localhost:7071/health | Health status endpoint |

## Service Architecture

The application uses .NET Aspire for orchestration with the following service topology:

```
┌─────────────────────────────────────────┐
│          .NET Aspire Host               │
│     (ProductManagementApp.Aspire)       │
└─────────────────┬───────────────────────┘
                  │
                  ├── PostgreSQL Container
                  │   ├── Image: postgres:15-alpine
                  │   ├── Database: productdb
                  │   ├── Port: 5432
                  │   └── Persistent data volume
                  │
                  ├── ProductAPI Service
                  │   ├── ASP.NET Core Web API
                  │   ├── FastEndpoints framework
                  │   ├── Port: 7071 (HTTPS), 5262 (HTTP)
                  │   ├── Service name: "productapi"
                  │   └── Dependencies: PostgreSQL
                  │
                  └── Product.Blazor Service
                      ├── Blazor Server Application
                      ├── Port: 5002 (HTTPS), 5003 (HTTP)
                      ├── Service name: "product-blazor"
                      └── Dependencies: ProductAPI
```

### Service Discovery
The application uses Microsoft.Extensions.ServiceDiscovery for automatic service resolution:
- Blazor app calls `http://productapi` instead of hardcoded URLs
- Service discovery resolves `productapi` to actual running endpoint
- Supports dynamic port assignment and load balancing

## Testing the Application

### Automated Testing
For comprehensive API testing, use the provided test scripts:

```bash
# Navigate to ProductAPI directory
cd src/ProductAPI

# Run automated test suite (Windows CMD)
TestAPI.cmd

# Or use PowerShell script
./StartStandalone.ps1
```

### Manual Testing Commands

**Health Check:**
```bash
curl http://localhost:5262/health
```

**Get All Products:**
```bash
curl http://localhost:5262/products
```

**Create a Product:**
```bash
curl -X POST http://localhost:5262/products ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Test Product\",\"price\":19.99,\"description\":\"Sample product\"}"
```

**Update a Product:**
```bash
curl -X PUT http://localhost:5262/products/1 ^
  -H "Content-Type: application/json" ^
  -d "{\"id\":1,\"name\":\"Updated Product\",\"price\":29.99,\"description\":\"Updated description\"}"
```

**Delete a Product:**
```bash
curl -X DELETE http://localhost:5262/products/1
```

### Using the Blazor Frontend
1. Navigate to https://localhost:5002
2. Use the web interface to:
   - View all products in a responsive table
   - Create new products with validation
   - Edit existing products
   - Delete products with confirmation
   - View detailed product information

## Monitoring and Observability

The Aspire Dashboard (https://localhost:17152) provides:

### Resources View
- Service health status and resource utilization
- Container status and logs
- Database connection health

### Distributed Tracing
- Request flow across services
- Performance bottleneck identification
- Error correlation across the application

### Centralized Logging
- Real-time logs from all services
- Filterable by service and log level
- Structured logging with correlation IDs

### Metrics Dashboard
- HTTP request rates and response times
- Database connection pool statistics
- Custom application metrics

## Configuration

### Environment Variables
Aspire automatically configures the following:

**For ProductAPI:**
```
ConnectionStrings__productdb=<PostgreSQL connection string>
ASPNETCORE_ENVIRONMENT=Development
```

**For Blazor App:**
```
services__productapi__https__0=<ProductAPI HTTPS endpoint>
services__productapi__http__0=<ProductAPI HTTP endpoint>
```

### Custom Configuration
Application-specific settings can be configured in respective `appsettings.json` files:

**ProductAPI Settings:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=productdb;Username=postgres;Password=Password123"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## Troubleshooting

### Common Issues and Solutions

**Application Won't Start:**
1. Verify Docker Desktop is running
2. Check if required ports (5002, 7071, 17152, 5432) are available
3. Ensure PostgreSQL container is running: `docker ps`
4. Run `dotnet clean` and `dotnet restore`

**Database Connection Issues:**
1. Verify PostgreSQL is accessible:
   ```bash
   docker exec -it postgres-productmgmt psql -U postgres -d productdb
   ```
2. Check connection string in appsettings.json
3. Ensure database exists and is properly initialized

**Service Discovery Not Working:**
1. Check that both services have `AddServiceDiscovery()` in Program.cs
2. Verify service names match between Aspire host and client applications
3. Check Aspire Dashboard for service registration status

**Build Errors:**
```bash
# Clean and restore packages
dotnet clean
dotnet restore
dotnet build

# If still issues, clear NuGet cache
dotnet nuget locals all --clear
```

**Port Conflicts:**
1. Stop other applications using required ports
2. Modify port assignments in `Properties/launchSettings.json` if needed
3. Use `netstat -ano | findstr :<port>` to identify conflicting processes

### Log Analysis
1. Open Aspire Dashboard at https://localhost:17152
2. Navigate to "Console Logs" tab
3. Filter by service name to isolate issues
4. Look for startup errors, connection failures, or exceptions

### Performance Issues
1. Check resource usage in Aspire Dashboard
2. Monitor database connection pool statistics
3. Review distributed tracing for slow requests
4. Verify adequate system resources (CPU, memory)

## Development Workflow

### Running Individual Services
For development, services can be run independently:

**ProductAPI Standalone:**
```bash
cd src/ProductAPI
./StartStandalone.ps1
```

**Blazor App Standalone:**
```bash
cd src/Product.Blazor
dotnet run
```

### Hot Reload
Both Blazor and API support hot reload for faster development:
- Changes to Razor components reflect immediately
- API endpoint changes require rebuild

### Database Schema Changes
The application automatically initializes the database schema on startup. For schema changes:
1. Modify the schema in `ProductRepository.EnsureDatabaseCreatedAsync()`
2. Restart the application
3. Or manually run database migrations

## Additional Resources

- **Detailed API Documentation**: `src/ProductAPI/QUICK_START.md`
- **Comprehensive Testing Guide**: `src/ProductAPI/TESTING_GUIDE.md`
- **FastEndpoints Documentation**: https://fast-endpoints.com/
- **.NET Aspire Documentation**: https://docs.microsoft.com/en-us/dotnet/aspire/
- **Blazor Server Documentation**: https://docs.microsoft.com/en-us/aspnet/core/blazor/
