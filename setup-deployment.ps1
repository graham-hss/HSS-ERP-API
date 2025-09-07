# HSS Training ERP API - Quick Deployment Setup Script
# This script helps set up the deployment environment and provides guided deployment

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("AzureCLI", "PowerShell", "Terraform")]
    [string]$DeploymentMethod = "AzureCLI",
    
    [Parameter(Mandatory=$false)]
    [string]$WebAppName = "hss-training-erp-api",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipPrerequisites
)

$ErrorActionPreference = "Stop"

# Colors for output
$Red = "Red"
$Green = "Green"
$Yellow = "Yellow"
$Blue = "Cyan"

function Write-ColorOutput {
    param([string]$Message, [string]$Color = "White")
    Write-Host $Message -ForegroundColor $Color
}

function Test-Prerequisites {
    Write-ColorOutput "Checking prerequisites..." $Blue
    
    $prerequisites = @()
    
    # Check Azure CLI
    try {
        $azVersion = az --version 2>$null | Select-String "azure-cli"
        if ($azVersion) {
            Write-ColorOutput "✅ Azure CLI installed: $($azVersion.ToString().Trim())" $Green
        } else {
            $prerequisites += "Azure CLI not found"
        }
    } catch {
        $prerequisites += "Azure CLI not found"
    }
    
    # Check .NET SDK
    try {
        $dotnetVersion = dotnet --version 2>$null
        if ($dotnetVersion -and $dotnetVersion -like "8.*") {
            Write-ColorOutput "✅ .NET 8 SDK installed: $dotnetVersion" $Green
        } else {
            $prerequisites += ".NET 8 SDK not found or wrong version"
        }
    } catch {
        $prerequisites += ".NET 8 SDK not found"
    }
    
    # Check Git
    try {
        $gitVersion = git --version 2>$null
        if ($gitVersion) {
            Write-ColorOutput "✅ Git installed: $gitVersion" $Green
        } else {
            $prerequisites += "Git not found"
        }
    } catch {
        $prerequisites += "Git not found"
    }
    
    # Check Terraform (if selected)
    if ($DeploymentMethod -eq "Terraform") {
        try {
            $terraformVersion = terraform --version 2>$null | Select-Object -First 1
            if ($terraformVersion) {
                Write-ColorOutput "✅ Terraform installed: $terraformVersion" $Green
            } else {
                $prerequisites += "Terraform not found (required for selected deployment method)"
            }
        } catch {
            $prerequisites += "Terraform not found (required for selected deployment method)"
        }
    }
    
    if ($prerequisites.Count -gt 0) {
        Write-ColorOutput "❌ Missing prerequisites:" $Red
        foreach ($prereq in $prerequisites) {
            Write-ColorOutput "   - $prereq" $Red
        }
        Write-ColorOutput "Please install missing prerequisites and run again." $Yellow
        return $false
    }
    
    Write-ColorOutput "✅ All prerequisites satisfied!" $Green
    return $true
}

function Test-AzureConnection {
    Write-ColorOutput "Checking Azure connection..." $Blue
    
    try {
        $account = az account show 2>$null | ConvertFrom-Json
        if ($account) {
            Write-ColorOutput "✅ Connected to Azure as: $($account.user.name)" $Green
            Write-ColorOutput "   Subscription: $($account.name) ($($account.id))" $Blue
            return $true
        }
    } catch {
        # Not connected
    }
    
    Write-ColorOutput "❌ Not connected to Azure" $Red
    Write-ColorOutput "Please run: az login" $Yellow
    return $false
}

function Test-WebAppNameAvailability {
    param([string]$Name)
    
    Write-ColorOutput "Checking Web App name availability..." $Blue
    
    try {
        $availability = az webapp list --query "[?name=='$Name']" | ConvertFrom-Json
        if ($availability.Count -eq 0) {
            # Try to check if the name is globally available
            $testUrl = "https://$Name.azurewebsites.net"
            try {
                $response = Invoke-WebRequest -Uri $testUrl -Method Head -TimeoutSec 5 -ErrorAction SilentlyContinue
                Write-ColorOutput "⚠️  Web App name '$Name' may already be in use globally" $Yellow
                return $false
            } catch {
                Write-ColorOutput "✅ Web App name '$Name' appears to be available" $Green
                return $true
            }
        } else {
            Write-ColorOutput "✅ Web App '$Name' already exists in your subscription" $Green
            return $true
        }
    } catch {
        Write-ColorOutput "⚠️  Unable to verify Web App name availability" $Yellow
        return $true  # Continue anyway
    }
}

function Start-Deployment {
    param([string]$Method)
    
    Write-ColorOutput "Starting deployment using $Method method..." $Blue
    
    switch ($Method) {
        "AzureCLI" {
            Write-ColorOutput "Executing Azure CLI deployment script..." $Blue
            & bash -c "./deploy-azure.sh"
        }
        "PowerShell" {
            Write-ColorOutput "Executing PowerShell deployment script..." $Blue
            $connectionString = "Server=hss-training.postgres.database.azure.com;Database=hsstraining;Port=5432;User Id=adminuser;Password=Gremlin@36;Ssl Mode=Require;Search Path=tms,public;"
            .\azure-deploy.ps1 -ResourceGroupName "hss-training-rg" -AppServicePlanName "hss-training-asp" -WebAppName $WebAppName -Location "East US" -PostgreSqlConnectionString $connectionString
        }
        "Terraform" {
            Write-ColorOutput "Executing Terraform deployment..." $Blue
            Set-Location "terraform"
            
            # Initialize if needed
            if (!(Test-Path ".terraform")) {
                terraform init
            }
            
            # Check if tfvars file exists
            if (!(Test-Path "terraform.tfvars")) {
                Write-ColorOutput "Creating terraform.tfvars from example..." $Yellow
                Copy-Item "terraform.tfvars.example" "terraform.tfvars"
                Write-ColorOutput "Please edit terraform/terraform.tfvars with your values before continuing." $Yellow
                Write-ColorOutput "Press any key when ready to continue..." $Yellow
                Read-Host
            }
            
            terraform plan -out="deployment.tfplan"
            
            Write-ColorOutput "Review the plan above. Continue with deployment? (y/N)" $Yellow
            $confirm = Read-Host
            if ($confirm -eq "y" -or $confirm -eq "Y") {
                terraform apply "deployment.tfplan"
            } else {
                Write-ColorOutput "Deployment cancelled by user." $Yellow
                return
            }
            
            Set-Location ".."
        }
    }
}

function Show-PostDeploymentSteps {
    Write-ColorOutput "`n🎉 Deployment completed! Next steps:" $Green
    Write-ColorOutput "===========================================" $Green
    
    Write-ColorOutput "1. Configure GitHub Actions:" $Blue
    Write-ColorOutput "   - Add AZURE_WEBAPP_PUBLISH_PROFILE to GitHub Secrets" $Blue
    Write-ColorOutput "   - Publish profile was saved to ./publish-profile.xml" $Blue
    
    Write-ColorOutput "2. Update Teams Frontend:" $Blue
    Write-ColorOutput "   - Change API base URL from https://localhost:7001" $Blue
    Write-ColorOutput "   - To: https://$WebAppName.azurewebsites.net" $Blue
    
    Write-ColorOutput "3. Test the deployment:" $Blue
    Write-ColorOutput "   - Health Check: https://$WebAppName.azurewebsites.net/health" $Blue
    Write-ColorOutput "   - Swagger UI: https://$WebAppName.azurewebsites.net/swagger" $Blue
    
    Write-ColorOutput "4. Monitor the application:" $Blue
    Write-ColorOutput "   - Check Azure Portal > App Services > $WebAppName" $Blue
    Write-ColorOutput "   - Review Application Insights logs" $Blue
    
    Write-ColorOutput "`nFor detailed instructions, see DEPLOYMENT.md" $Yellow
}

# Main execution
Write-ColorOutput "HSS Training ERP API - Deployment Setup" $Green
Write-ColorOutput "=======================================" $Green
Write-ColorOutput "Deployment Method: $DeploymentMethod" $Blue
Write-ColorOutput "Web App Name: $WebAppName" $Blue

if (!$SkipPrerequisites) {
    if (!(Test-Prerequisites)) {
        exit 1
    }
    
    if (!(Test-AzureConnection)) {
        Write-ColorOutput "Please run 'az login' first and then re-run this script." $Yellow
        exit 1
    }
    
    Test-WebAppNameAvailability $WebAppName
}

Write-ColorOutput "`nReady to deploy. Continue? (y/N)" $Yellow
$confirm = Read-Host
if ($confirm -ne "y" -and $confirm -ne "Y") {
    Write-ColorOutput "Deployment cancelled by user." $Yellow
    exit 0
}

try {
    Start-Deployment $DeploymentMethod
    Show-PostDeploymentSteps
} catch {
    Write-ColorOutput "❌ Deployment failed: $($_.Exception.Message)" $Red
    Write-ColorOutput "Check the logs above for details." $Yellow
    exit 1
}

Write-ColorOutput "`n✅ Setup completed successfully!" $Green