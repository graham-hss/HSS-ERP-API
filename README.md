# HSS Training ERP Backend API

A comprehensive .NET 8 Web API serving as the backend microservice for the HSS Training ERP system, designed for Teams integration and Azure deployment.

## Features

- **.NET 8 Web API** with Entity Framework Core
- **PostgreSQL** database integration with Npgsql
- **Teams-optimized CORS** configuration
- **Application Insights** monitoring and telemetry
- **Health checks** and diagnostics
- **Swagger/OpenAPI** documentation
- **JWT Authentication** ready (configurable)
- **Microservice architecture** with configurable service implementations

## Quick Start

### Local Development

1. **Clone and navigate to the project**:
   ```bash
   cd D:\Projects\HSSTraining\HSS.ERP.API
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Run the application**:
   ```bash
   dotnet run --project HSS.ERP.API/HSS.ERP.API.csproj --urls=https://localhost:7001
   ```

4. **Access endpoints**:
   - API: https://localhost:7001
   - Swagger: https://localhost:7001/swagger
   - Health: https://localhost:7001/health

### Azure Deployment (Quick Setup)

**Option 1: Automated Setup (Recommended)**
```powershell
.\setup-deployment.ps1 -DeploymentMethod AzureCLI
```

**Option 2: Azure CLI**
```bash
chmod +x deploy-azure.sh
./deploy-azure.sh
```

**Option 3: Terraform**
```bash
cd terraform
terraform init
cp terraform.tfvars.example terraform.tfvars
# Edit terraform.tfvars with your values
terraform apply
```

## Production URLs

After deployment, your API will be available at:

- **Primary API**: https://hss-training-erp-api.azurewebsites.net
- **Health Check**: https://hss-training-erp-api.azurewebsites.net/health
- **Swagger Documentation**: https://hss-training-erp-api.azurewebsites.net/swagger
- **Staging Slot** (if enabled): https://hss-training-erp-api-staging.azurewebsites.net

## Project Structure

```
HSS.ERP.API/
├── HSS.ERP.API/
│   ├── Controllers/          # API controllers
│   ├── Data/                 # Entity Framework DbContext
│   ├── Models/               # Data models and DTOs
│   ├── Services/             # Business logic services
│   ├── Middleware/           # Custom middleware
│   ├── Migrations/           # EF Core migrations
│   └── Program.cs            # Application entry point
├── .github/workflows/        # GitHub Actions CI/CD
├── terraform/                # Infrastructure as Code
├── deploy-azure.sh           # Azure CLI deployment
├── azure-deploy.ps1          # PowerShell deployment
├── setup-deployment.ps1      # Automated setup
└── DEPLOYMENT.md             # Detailed deployment guide
```

## API Endpoints

### Core Operations
- `GET /health` - Application health status
- `GET /swagger` - Interactive API documentation

### Business Operations
- `GET /api/customers` - Customer management
- `GET /api/invoices` - Invoice operations  
- `GET /api/bookings` - Training booking management
- `GET /api/courses` - Course catalog
- `GET /api/stock` - Inventory management
- `GET /api/tms` - TMS integration endpoints

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development` |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection | See appsettings.json |
| `UseApiServices` | Enable API service implementations | `false` |
| `ApplicationInsights__Enabled` | Enable monitoring | `false` |
| `TeamsApp__ProductionUrl` | Teams app URL for CORS | - |

### Database Connection

The API connects to the existing PostgreSQL instance:
```
Server: hss-training.postgres.database.azure.com
Database: hsstraining
Schema: tms, public
```

## Teams Integration

### CORS Configuration

Pre-configured to allow requests from:
- Microsoft Teams environments (`*.teams.microsoft.com`)
- Local development (`localhost:44302`, `localhost:53000`)
- Configurable production Teams app URL

### Authentication

- JWT Bearer authentication configured but not enforced
- Ready for Azure AD integration
- Supports Teams SSO token validation

## CI/CD Pipeline

### GitHub Actions Workflow

The deployment pipeline includes:

1. **Build & Test**: Restore, build, and test the application
2. **Security Scan**: Dependency and vulnerability scanning
3. **Deploy**: Automated deployment to Azure Web App
4. **Health Check**: Post-deployment verification
5. **Staging Support**: Blue-green deployment capability

### Required Secrets

Add to GitHub repository settings:
- `AZURE_WEBAPP_PUBLISH_PROFILE`: Azure Web App publish profile

### Triggers

- Push to `main`/`master` branch
- Pull requests (build and test only)
- Manual workflow dispatch

## Monitoring

### Application Insights

Comprehensive monitoring including:
- Request/response metrics
- Exception tracking
- Performance monitoring  
- Custom telemetry events
- Log aggregation

### Health Checks

- Database connectivity verification
- Endpoint: `/health`
- Azure App Service health monitoring integration

## Security

### HTTPS Enforcement
- All traffic redirected to HTTPS
- TLS 1.2 minimum version
- HSTS headers enabled

### Database Security
- SSL/TLS encrypted connections
- Parameterized queries (EF Core)
- Connection string encryption in Azure

### CORS Policy
- Restrictive origin policy
- Credentials support for authenticated requests
- Wildcard subdomain support for Teams

## Scaling

### Current Configuration
- **App Service Plan**: Basic B1 (1 vCPU, 1.75 GB RAM)
- **Estimated Cost**: ~$13-18/month including Application Insights

### Scaling Options
- **Vertical**: Scale up to higher SKUs (S1, P1v2, etc.)
- **Horizontal**: Auto-scaling with Standard tier and above
- **Geographic**: Deploy to multiple regions

## Development

### Prerequisites
- .NET 8 SDK
- PostgreSQL access
- Visual Studio 2022 or VS Code

### Building
```bash
dotnet build HSS.ERP.API.sln --configuration Release
```

### Testing
```bash
dotnet test HSS.ERP.API.sln --configuration Release
```

### Database Migrations
```bash
dotnet ef migrations add MigrationName --project HSS.ERP.API
dotnet ef database update --project HSS.ERP.API
```

## Support

### Documentation
- **Deployment Guide**: [DEPLOYMENT.md](DEPLOYMENT.md)
- **API Documentation**: Available at `/swagger` endpoint
- **Architecture**: Microservice-based with configurable implementations

### Troubleshooting
- Check Application Insights logs
- Review Azure App Service diagnostics  
- Verify database connectivity
- Test CORS configuration

### Contact
- Repository: https://github.com/graham-hss/HSS-ERP-API
- Teams Integration: Part of HSS Training ERP system

---

**Version**: 1.0.0  
**Last Updated**: September 2024  
**Target Framework**: .NET 8.0