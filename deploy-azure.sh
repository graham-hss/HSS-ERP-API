#!/bin/bash

# Azure CLI Deployment Script for HSS Training ERP Backend API
# This script creates and configures Azure resources using Azure CLI

set -e

# Configuration variables
RESOURCE_GROUP_NAME="hss-training-rg"
APP_SERVICE_PLAN_NAME="hss-training-asp"
WEB_APP_NAME="hss-training-erp-api"
LOCATION="East US"
POSTGRESQL_CONNECTION_STRING="Server=hss-training.postgres.database.azure.com;Database=hsstraining;Port=5432;User Id=adminuser;Password=Gremlin@36;Ssl Mode=Require;Search Path=tms,public;"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${GREEN}HSS Training ERP Backend API - Azure Deployment${NC}"
echo -e "${GREEN}=================================================${NC}"

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo -e "${RED}Azure CLI is not installed. Please install it first.${NC}"
    echo "Visit: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

# Login to Azure
echo -e "${YELLOW}Checking Azure login status...${NC}"
if ! az account show &> /dev/null; then
    echo -e "${YELLOW}Please log in to Azure...${NC}"
    az login
fi

# Display current subscription
SUBSCRIPTION_NAME=$(az account show --query "name" -o tsv)
SUBSCRIPTION_ID=$(az account show --query "id" -o tsv)
echo -e "${BLUE}Using subscription: ${SUBSCRIPTION_NAME} (${SUBSCRIPTION_ID})${NC}"

# Create resource group if it doesn't exist
echo -e "${YELLOW}Creating resource group: ${RESOURCE_GROUP_NAME}${NC}"
az group create \
    --name $RESOURCE_GROUP_NAME \
    --location "$LOCATION" \
    --output table

# Create App Service Plan if it doesn't exist
echo -e "${YELLOW}Creating App Service Plan: ${APP_SERVICE_PLAN_NAME}${NC}"
az appservice plan create \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $APP_SERVICE_PLAN_NAME \
    --location "$LOCATION" \
    --sku B1 \
    --is-linux \
    --output table

# Create Web App
echo -e "${YELLOW}Creating Web App: ${WEB_APP_NAME}${NC}"
az webapp create \
    --resource-group $RESOURCE_GROUP_NAME \
    --plan $APP_SERVICE_PLAN_NAME \
    --name $WEB_APP_NAME \
    --runtime "DOTNETCORE:8.0" \
    --output table

# Configure App Settings
echo -e "${YELLOW}Configuring Web App settings...${NC}"

# Set connection string
az webapp config connection-string set \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $WEB_APP_NAME \
    --settings DefaultConnection="$POSTGRESQL_CONNECTION_STRING" \
    --connection-string-type PostgreSQL

# Set app settings
az webapp config appsettings set \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $WEB_APP_NAME \
    --settings \
        ASPNETCORE_ENVIRONMENT="Production" \
        WEBSITES_ENABLE_APP_SERVICE_STORAGE="false" \
        UseApiServices="false" \
        ApplicationInsights__Enabled="true" \
        AllowedHosts="*" \
        SCM_DO_BUILD_DURING_DEPLOYMENT="true" \
        ENABLE_ORYX_BUILD="true"

# Enable Application Insights
echo -e "${YELLOW}Enabling Application Insights...${NC}"
az monitor app-insights component create \
    --app $WEB_APP_NAME \
    --location "$LOCATION" \
    --resource-group $RESOURCE_GROUP_NAME \
    --output table || echo "Application Insights may already exist or creation failed"

# Get Application Insights connection string
AI_CONNECTION_STRING=$(az monitor app-insights component show \
    --app $WEB_APP_NAME \
    --resource-group $RESOURCE_GROUP_NAME \
    --query "connectionString" \
    --output tsv 2>/dev/null || echo "")

if [ ! -z "$AI_CONNECTION_STRING" ]; then
    az webapp config appsettings set \
        --resource-group $RESOURCE_GROUP_NAME \
        --name $WEB_APP_NAME \
        --settings ApplicationInsights__ConnectionString="$AI_CONNECTION_STRING"
fi

# Configure deployment source (for GitHub Actions)
echo -e "${YELLOW}Configuring deployment settings...${NC}"
az webapp config set \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $WEB_APP_NAME \
    --always-on true \
    --auto-heal-enabled true \
    --http20-enabled true \
    --min-tls-version "1.2"

# Enable detailed error messages for debugging
az webapp config set \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $WEB_APP_NAME \
    --detailed-error-logging-enabled true \
    --failed-request-tracing-enabled true \
    --http-logging-enabled true

# Configure CORS for Teams integration
echo -e "${YELLOW}Configuring CORS settings...${NC}"
az webapp cors add \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $WEB_APP_NAME \
    --allowed-origins \
        "https://*.teams.microsoft.com" \
        "https://*.microsoft.com" \
        "https://localhost:44302" \
        "https://localhost:53000"

# Get publish profile for GitHub Actions
echo -e "${YELLOW}Generating publish profile for GitHub Actions...${NC}"
PUBLISH_PROFILE=$(az webapp deployment list-publishing-profiles \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $WEB_APP_NAME \
    --xml)

# Save publish profile to file
echo "$PUBLISH_PROFILE" > ./publish-profile.xml
echo -e "${BLUE}Publish profile saved to: publish-profile.xml${NC}"

# Display URLs
WEB_APP_URL="https://${WEB_APP_NAME}.azurewebsites.net"
SWAGGER_URL="${WEB_APP_URL}/swagger"
HEALTH_URL="${WEB_APP_URL}/health"

echo -e "${GREEN}"
echo "Deployment completed successfully!"
echo "======================================"
echo "Web App Name: $WEB_APP_NAME"
echo "Resource Group: $RESOURCE_GROUP_NAME"
echo "Web App URL: $WEB_APP_URL"
echo "Swagger URL: $SWAGGER_URL"
echo "Health Check: $HEALTH_URL"
echo ""
echo "Next Steps:"
echo "1. Add the publish profile content to GitHub Secrets as 'AZURE_WEBAPP_PUBLISH_PROFILE'"
echo "2. Update your GitHub Actions workflow if needed"
echo "3. Push your code to trigger the deployment"
echo "4. Update your Teams frontend to use the production API URL"
echo -e "${NC}"

# Test connectivity
echo -e "${YELLOW}Testing basic connectivity...${NC}"
if curl -f -s --max-time 10 "$WEB_APP_URL" > /dev/null 2>&1; then
    echo -e "${GREEN}✅ Web App is accessible${NC}"
else
    echo -e "${YELLOW}⏳ Web App may still be starting up or needs deployment${NC}"
fi

echo -e "${BLUE}Deployment script completed!${NC}"