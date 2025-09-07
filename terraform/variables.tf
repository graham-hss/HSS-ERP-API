# Terraform Variables for HSS Training ERP Backend API

variable "resource_group_name" {
  description = "The name of the resource group where resources will be created"
  type        = string
  default     = "hss-training-rg"
  
  validation {
    condition     = can(regex("^[a-zA-Z0-9._-]+$", var.resource_group_name))
    error_message = "Resource group name must contain only letters, numbers, periods, underscores, and hyphens."
  }
}

variable "location" {
  description = "The Azure region where all resources will be created"
  type        = string
  default     = "East US"
  
  validation {
    condition = contains([
      "East US", "East US 2", "West US", "West US 2", "West US 3",
      "Central US", "North Central US", "South Central US", "West Central US",
      "Canada Central", "Canada East", "UK South", "UK West",
      "West Europe", "North Europe", "France Central", "Germany West Central",
      "Switzerland North", "Norway East", "Sweden Central"
    ], var.location)
    error_message = "Location must be a valid Azure region."
  }
}

variable "web_app_name" {
  description = "The name of the web app - must be globally unique"
  type        = string
  default     = "hss-training-erp-api"
  
  validation {
    condition     = can(regex("^[a-zA-Z0-9][a-zA-Z0-9-]*[a-zA-Z0-9]$", var.web_app_name)) && length(var.web_app_name) >= 2 && length(var.web_app_name) <= 60
    error_message = "Web app name must be 2-60 characters, start and end with alphanumeric characters, and can contain hyphens."
  }
}

variable "app_service_plan_name" {
  description = "The name of the app service plan"
  type        = string
  default     = "hss-training-asp"
  
  validation {
    condition     = can(regex("^[a-zA-Z0-9-]+$", var.app_service_plan_name))
    error_message = "App service plan name must contain only letters, numbers, and hyphens."
  }
}

variable "sku_name" {
  description = "The SKU name for the App Service Plan"
  type        = string
  default     = "B1"
  
  validation {
    condition = contains([
      "F1", "D1", "B1", "B2", "B3",
      "S1", "S2", "S3", "P1", "P2", "P3",
      "P1v2", "P2v2", "P3v2", "P1v3", "P2v3", "P3v3"
    ], var.sku_name)
    error_message = "SKU name must be a valid App Service Plan SKU."
  }
}

variable "postgresql_connection_string" {
  description = "PostgreSQL connection string for the database"
  type        = string
  sensitive   = true
  
  validation {
    condition     = can(regex("^Server=.*Database=.*User Id=.*Password=.*", var.postgresql_connection_string))
    error_message = "PostgreSQL connection string must include Server, Database, User Id, and Password parameters."
  }
}

variable "environment" {
  description = "Environment name (development, staging, production)"
  type        = string
  default     = "production"
  
  validation {
    condition     = contains(["development", "staging", "production"], var.environment)
    error_message = "Environment must be either development, staging, or production."
  }
}

variable "enable_staging_slot" {
  description = "Whether to create a staging deployment slot"
  type        = bool
  default     = true
}

variable "teams_app_production_url" {
  description = "Production URL of the Teams application for CORS configuration"
  type        = string
  default     = ""
  
  validation {
    condition     = var.teams_app_production_url == "" || can(regex("^https://", var.teams_app_production_url))
    error_message = "Teams app production URL must start with https:// if provided."
  }
}

variable "enable_application_insights" {
  description = "Whether to enable Application Insights monitoring"
  type        = bool
  default     = true
}

variable "log_retention_days" {
  description = "Number of days to retain logs"
  type        = number
  default     = 7
  
  validation {
    condition     = var.log_retention_days >= 1 && var.log_retention_days <= 90
    error_message = "Log retention days must be between 1 and 90."
  }
}

variable "health_check_path" {
  description = "Health check endpoint path"
  type        = string
  default     = "/health"
  
  validation {
    condition     = can(regex("^/", var.health_check_path))
    error_message = "Health check path must start with a forward slash."
  }
}

variable "always_on" {
  description = "Keep the app loaded even when there's no traffic"
  type        = bool
  default     = true
}

variable "https_only" {
  description = "Only allow HTTPS traffic"
  type        = bool
  default     = true
}

variable "tags" {
  description = "A map of tags to assign to the resources"
  type        = map(string)
  default = {
    Project     = "HSS-Training-ERP"
    Component   = "Backend-API"
    ManagedBy   = "Terraform"
  }
}