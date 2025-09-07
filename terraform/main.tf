# Azure Provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }

  # Backend configuration for storing state
  # Uncomment and configure for production use
  # backend "azurerm" {
  #   resource_group_name  = "terraform-state-rg"
  #   storage_account_name = "terraformstate"
  #   container_name       = "tfstate"
  #   key                  = "hss-training-erp-api.tfstate"
  # }
}

# Configure the Azure Provider
provider "azurerm" {
  features {}
}

# Variables
variable "resource_group_name" {
  description = "The name of the resource group"
  type        = string
  default     = "hss-training-rg"
}

variable "location" {
  description = "The Azure location where all resources will be created"
  type        = string
  default     = "East US"
}

variable "web_app_name" {
  description = "The name of the web app"
  type        = string
  default     = "hss-training-erp-api"
}

variable "app_service_plan_name" {
  description = "The name of the app service plan"
  type        = string
  default     = "hss-training-asp"
}

variable "postgresql_connection_string" {
  description = "PostgreSQL connection string"
  type        = string
  sensitive   = true
}

variable "environment" {
  description = "Environment name"
  type        = string
  default     = "production"
}

# Data source for existing resource group (if it exists)
data "azurerm_resource_group" "main" {
  name = var.resource_group_name
}

# App Service Plan
resource "azurerm_service_plan" "main" {
  name                = var.app_service_plan_name
  resource_group_name = data.azurerm_resource_group.main.name
  location            = data.azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = "B1"

  tags = {
    Environment = var.environment
    Project     = "HSS-Training-ERP"
    Component   = "Backend-API"
  }
}

# Application Insights
resource "azurerm_application_insights" "main" {
  name                = "${var.web_app_name}-ai"
  location            = data.azurerm_resource_group.main.location
  resource_group_name = data.azurerm_resource_group.main.name
  application_type    = "web"

  tags = {
    Environment = var.environment
    Project     = "HSS-Training-ERP"
    Component   = "Monitoring"
  }
}

# Web App
resource "azurerm_linux_web_app" "main" {
  name                = var.web_app_name
  resource_group_name = data.azurerm_resource_group.main.name
  location            = data.azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.main.id

  https_only = true

  site_config {
    always_on         = true
    health_check_path = "/health"
    
    application_stack {
      dotnet_version = "8.0"
    }

    cors {
      allowed_origins = [
        "https://*.teams.microsoft.com",
        "https://*.microsoft.com",
        "https://teams.microsoft.com",
        "https://localhost:44302",
        "https://localhost:53000"
      ]
      support_credentials = true
    }
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"                = "Production"
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE"   = "false"
    "UseApiServices"                        = "false"
    "ApplicationInsights__Enabled"          = "true"
    "ApplicationInsights__ConnectionString" = azurerm_application_insights.main.connection_string
    "AllowedHosts"                         = "*"
    "SCM_DO_BUILD_DURING_DEPLOYMENT"      = "true"
    "ENABLE_ORYX_BUILD"                    = "true"
  }

  connection_string {
    name  = "DefaultConnection"
    type  = "PostgreSQL"
    value = var.postgresql_connection_string
  }

  logs {
    detailed_error_messages = true
    failed_request_tracing  = true
    
    http_logs {
      file_system {
        retention_in_days = 7
        retention_in_mb   = 50
      }
    }
  }

  tags = {
    Environment = var.environment
    Project     = "HSS-Training-ERP"
    Component   = "Backend-API"
  }
}

# Web App Slot for staging (optional)
resource "azurerm_linux_web_app_slot" "staging" {
  count          = var.environment == "production" ? 1 : 0
  name           = "staging"
  app_service_id = azurerm_linux_web_app.main.id

  site_config {
    always_on         = false
    health_check_path = "/health"
    
    application_stack {
      dotnet_version = "8.0"
    }
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"                = "Staging"
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE"   = "false"
    "UseApiServices"                        = "false"
    "ApplicationInsights__Enabled"          = "true"
    "ApplicationInsights__ConnectionString" = azurerm_application_insights.main.connection_string
    "AllowedHosts"                         = "*"
  }

  connection_string {
    name  = "DefaultConnection"
    type  = "PostgreSQL"
    value = var.postgresql_connection_string
  }

  tags = {
    Environment = "staging"
    Project     = "HSS-Training-ERP"
    Component   = "Backend-API"
  }
}

# Output values
output "web_app_url" {
  description = "The URL of the web app"
  value       = "https://${azurerm_linux_web_app.main.name}.azurewebsites.net"
}

output "web_app_name" {
  description = "The name of the web app"
  value       = azurerm_linux_web_app.main.name
}

output "staging_url" {
  description = "The URL of the staging slot"
  value       = var.environment == "production" ? "https://${azurerm_linux_web_app.main.name}-staging.azurewebsites.net" : null
}

output "application_insights_instrumentation_key" {
  description = "Application Insights instrumentation key"
  value       = azurerm_application_insights.main.instrumentation_key
  sensitive   = true
}

output "application_insights_connection_string" {
  description = "Application Insights connection string"
  value       = azurerm_application_insights.main.connection_string
  sensitive   = true
}

output "resource_group_name" {
  description = "The name of the resource group"
  value       = data.azurerm_resource_group.main.name
}