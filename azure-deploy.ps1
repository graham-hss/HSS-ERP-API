# Azure Deployment Script for HSS Training ERP Backend API
# This script creates the necessary Azure resources for the API deployment

param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$AppServicePlanName,
    
    [Parameter(Mandatory=$true)]
    [string]$WebAppName,
    
    [Parameter(Mandatory=$true)]
    [string]$Location = "East US",
    
    [Parameter(Mandatory=$true)]
    [string]$PostgreSqlConnectionString,
    
    [string]$ApplicationInsightsConnectionString = "",
    
    [string]$SubscriptionId = ""
)

# Login to Azure (if not already logged in)
try {
    $context = Get-AzContext
    if (!$context) {
        Write-Host "Please log in to Azure..."
        Connect-AzAccount
    }
} catch {
    Write-Host "Please log in to Azure..."
    Connect-AzAccount
}

# Set subscription if provided
if ($SubscriptionId) {
    Set-AzContext -SubscriptionId $SubscriptionId
}

Write-Host "Creating Azure resources for HSS Training ERP Backend API..." -ForegroundColor Green

# Check if resource group exists, create if it doesn't
$rg = Get-AzResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue
if (!$rg) {
    Write-Host "Creating resource group: $ResourceGroupName" -ForegroundColor Yellow
    New-AzResourceGroup -Name $ResourceGroupName -Location $Location
} else {
    Write-Host "Using existing resource group: $ResourceGroupName" -ForegroundColor Green
}

# Check if App Service Plan exists, create if it doesn't
$asp = Get-AzAppServicePlan -ResourceGroupName $ResourceGroupName -Name $AppServicePlanName -ErrorAction SilentlyContinue
if (!$asp) {
    Write-Host "Creating App Service Plan: $AppServicePlanName" -ForegroundColor Yellow
    New-AzAppServicePlan -ResourceGroupName $ResourceGroupName -Name $AppServicePlanName -Location $Location -Tier "Basic" -NumberofWorkers 1 -WorkerSize "Small"
} else {
    Write-Host "Using existing App Service Plan: $AppServicePlanName" -ForegroundColor Green
}

# Create Web App
Write-Host "Creating Web App: $WebAppName" -ForegroundColor Yellow
try {
    $webapp = Get-AzWebApp -ResourceGroupName $ResourceGroupName -Name $WebAppName -ErrorAction SilentlyContinue
    if (!$webapp) {
        $webapp = New-AzWebApp -ResourceGroupName $ResourceGroupName -Name $WebAppName -AppServicePlan $AppServicePlanName -Location $Location
        Write-Host "Web App created successfully: $WebAppName" -ForegroundColor Green
    } else {
        Write-Host "Web App already exists: $WebAppName" -ForegroundColor Green
    }
} catch {
    Write-Error "Failed to create Web App: $($_.Exception.Message)"
    exit 1
}

# Configure Web App Settings
Write-Host "Configuring Web App settings..." -ForegroundColor Yellow

$appSettings = @{
    "ASPNETCORE_ENVIRONMENT" = "Production"
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "ConnectionStrings__DefaultConnection" = $PostgreSqlConnectionString
    "UseApiServices" = "false"
    "ApplicationInsights__Enabled" = if ($ApplicationInsightsConnectionString) { "true" } else { "false" }
    "AllowedHosts" = "*"
}

if ($ApplicationInsightsConnectionString) {
    $appSettings["ApplicationInsights__ConnectionString"] = $ApplicationInsightsConnectionString
    $appSettings["APPINSIGHTS_INSTRUMENTATIONKEY"] = ""
}

# Apply app settings
Set-AzWebApp -ResourceGroupName $ResourceGroupName -Name $WebAppName -AppSettings $appSettings

# Configure deployment settings
Write-Host "Configuring deployment settings..." -ForegroundColor Yellow

# Set deployment source to GitHub (this will be configured later in GitHub Actions)
# Enable continuous deployment logging
Set-AzWebApp -ResourceGroupName $ResourceGroupName -Name $WebAppName -NetFrameworkVersion "v8.0" -Use32BitWorkerProcess $false

Write-Host "Azure resources created successfully!" -ForegroundColor Green
Write-Host "Web App URL: https://$WebAppName.azurewebsites.net" -ForegroundColor Cyan
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Configure GitHub Actions workflow with the deployment credentials" -ForegroundColor White
Write-Host "2. Add the Web App URL to your Teams frontend CORS configuration" -ForegroundColor White
Write-Host "3. Update your GitHub repository secrets with Azure credentials" -ForegroundColor White