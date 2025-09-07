# HSS Training ERP Backend API - Azure Deployment Guide

## Overview

This guide provides step-by-step instructions for deploying the HSS Training ERP Backend API to Microsoft Azure using multiple deployment methods including Azure CLI, PowerShell, and Terraform.

## Prerequisites

### Required Tools
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) (version 2.37.0 or later)
- [Azure PowerShell](https://docs.microsoft.com/en-us/powershell/azure/install-az-ps) (optional, for PowerShell script)
- [Terraform](https://www.terraform.io/downloads.html) (version 1.0 or later, optional for Infrastructure as Code)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### Azure Requirements
- Active Azure subscription
- Contributor or Owner role on the subscription or resource group
- Existing PostgreSQL server: `hss-training.postgres.database.azure.com`

## Architecture Overview

The deployment creates the following Azure resources:
- **Resource Group**: `hss-training-rg`
- **App Service Plan**: `hss-training-asp` (Basic B1 tier)
- **Web App**: `hss-training-erp-api`
- **Application Insights**: Monitoring and telemetry
- **Staging Slot**: Optional deployment slot for blue-green deployments

## Deployment Methods

### Method 1: Azure CLI Script (Recommended)

1. **Login to Azure**:
   ```bash
   az login
   ```

2. **Set your subscription** (if you have multiple):
   ```bash
   az account set --subscription "Your Subscription Name"
   ```

3. **Run the deployment script**:
   ```bash
   cd /path/to/HSS.ERP.API
   chmod +x deploy-azure.sh
   ./deploy-azure.sh
   ```

4. **Review the output** for the Web App URL and next steps.

### Method 2: Azure PowerShell Script

1. **Login to Azure**:
   ```powershell
   Connect-AzAccount
   ```

2. **Run the PowerShell deployment script**:
   ```powershell
   .\azure-deploy.ps1 -ResourceGroupName "hss-training-rg" -AppServicePlanName "hss-training-asp" -WebAppName "hss-training-erp-api" -Location "East US" -PostgreSqlConnectionString "Your-Connection-String"
   ```

### Method 3: Terraform (Infrastructure as Code)

1. **Initialize Terraform**:
   ```bash
   cd terraform
   terraform init
   ```

2. **Create terraform.tfvars file**:
   ```bash
   cp terraform.tfvars.example terraform.tfvars
   # Edit terraform.tfvars with your values
   ```

3. **Plan the deployment**:
   ```bash
   terraform plan
   ```

4. **Apply the configuration**:
   ```bash
   terraform apply
   ```

## GitHub Actions CI/CD Setup

### 1. Repository Secrets Configuration

Add the following secrets to your GitHub repository (`Settings > Secrets and variables > Actions`):

#### Required Secrets:
- `AZURE_WEBAPP_PUBLISH_PROFILE`: Download from Azure Portal under your Web App's Deployment Center

#### Optional Secrets (for enhanced security):
- `AZURE_CLIENT_ID`: Service Principal Client ID
- `AZURE_CLIENT_SECRET`: Service Principal Client Secret  
- `AZURE_TENANT_ID`: Azure Tenant ID
- `AZURE_SUBSCRIPTION_ID`: Azure Subscription ID

### 2. Get Publish Profile

```bash
# Using Azure CLI
az webapp deployment list-publishing-profiles \
  --resource-group hss-training-rg \
  --name hss-training-erp-api \
  --xml > publish-profile.xml
```

Copy the entire content of `publish-profile.xml` and add it as the `AZURE_WEBAPP_PUBLISH_PROFILE` secret.

### 3. Workflow Trigger

The GitHub Actions workflow (`.github/workflows/azure-deploy.yml`) triggers on:
- Push to `main` or `master` branch
- Pull requests to `main` or `master` branch
- Manual workflow dispatch

## Configuration

### Environment Variables

The following environment variables are configured automatically during deployment:

| Variable | Value | Description |
|----------|-------|-------------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Runtime environment |
| `ConnectionStrings__DefaultConnection` | `[PostgreSQL Connection String]` | Database connection |
| `UseApiServices` | `false` | Service implementation selection |
| `ApplicationInsights__Enabled` | `true` | Enable monitoring |
| `AllowedHosts` | `*` | Allowed host headers |

### CORS Configuration

The API is pre-configured with CORS settings to allow requests from:
- `https://*.teams.microsoft.com`
- `https://*.microsoft.com`
- `https://teams.microsoft.com`
- `https://localhost:44302` (local Teams development)
- `https://localhost:53000` (alternative local port)

## Production URL

Once deployed, your API will be available at:
- **Primary URL**: `https://hss-training-erp-api.azurewebsites.net`
- **Health Check**: `https://hss-training-erp-api.azurewebsites.net/health`
- **Swagger UI**: `https://hss-training-erp-api.azurewebsites.net/swagger`
- **Staging Slot**: `https://hss-training-erp-api-staging.azurewebsites.net` (if enabled)

## API Endpoints

### Core Endpoints
- `GET /health` - Health check endpoint
- `GET /swagger` - API documentation

### Business Endpoints
- `GET /api/customers` - Customer management
- `GET /api/invoices` - Invoice operations
- `GET /api/bookings` - Booking management
- `GET /api/courses` - Course catalog
- `GET /api/stock` - Inventory management

## Monitoring and Logging

### Application Insights
- **Metrics**: Request/response times, success rates, exceptions
- **Logs**: Application logs, trace information
- **Alerts**: Configurable alerts for errors and performance

### Health Checks
- Database connectivity check at `/health`
- Automatic health monitoring by Azure App Service

## Security Considerations

### HTTPS Enforcement
- All traffic is redirected to HTTPS
- TLS 1.2 minimum version enforced

### Database Security
- SSL/TLS encrypted connections to PostgreSQL
- Connection strings stored securely in Azure configuration

### Authentication (Future)
- JWT Bearer token authentication configured but not enforced
- Ready for Azure AD integration

## Scaling and Performance

### Vertical Scaling
- Current plan: Basic B1 (1 vCPU, 1.75 GB RAM)
- Can be scaled up through Azure Portal or CLI:
  ```bash
  az appservice plan update --resource-group hss-training-rg --name hss-training-asp --sku S1
  ```

### Horizontal Scaling
- Available with Standard tier and above
- Auto-scaling rules can be configured based on CPU, memory, or custom metrics

## Troubleshooting

### Common Issues

1. **Deployment Fails**
   - Check GitHub Actions logs
   - Verify publish profile is valid
   - Ensure resource names are unique globally

2. **Database Connection Issues**
   - Verify PostgreSQL server allows Azure connections
   - Check connection string format
   - Ensure database user has appropriate permissions

3. **CORS Errors**
   - Verify Teams app URL is included in CORS policy
   - Check browser console for specific CORS errors
   - Ensure requests include proper headers

### Log Access

```bash
# Stream live logs
az webapp log tail --resource-group hss-training-rg --name hss-training-erp-api

# Download log files
az webapp log download --resource-group hss-training-rg --name hss-training-erp-api
```

## Rollback Procedures

### Using Deployment Slots
```bash
# Swap staging and production slots
az webapp deployment slot swap --resource-group hss-training-rg --name hss-training-erp-api --slot staging --target-slot production
```

### Revert to Previous Version
```bash
# List deployments
az webapp deployment list --resource-group hss-training-rg --name hss-training-erp-api

# Redeploy specific version
az webapp deployment source sync --resource-group hss-training-rg --name hss-training-erp-api
```

## Maintenance

### Regular Tasks
- Monitor Application Insights for performance issues
- Review and rotate connection strings quarterly
- Update dependencies and security patches
- Monitor costs and optimize resource usage

### Backup Strategy
- PostgreSQL automated backups (managed by Azure)
- Application configuration backed up in Git
- Infrastructure as Code with Terraform state management

## Cost Optimization

### Current Estimated Costs (East US)
- App Service Plan (Basic B1): ~$13/month
- Application Insights: Pay-per-use (typically <$5/month for small apps)
- Data transfer: Minimal for API usage

### Optimization Tips
- Use staging slots efficiently (only when needed)
- Monitor and right-size the App Service Plan
- Implement caching to reduce database load
- Consider reserved instances for predictable workloads

## Support and Contact

For deployment issues or questions:
- Check the GitHub repository issues
- Review Azure Portal diagnostics
- Contact the HSS Training development team

---

**Last Updated**: September 2024
**Version**: 1.0.0