#!/usr/bin/env pwsh

# ========================================
# ProductAPI Standalone Launcher
# ========================================
# Quick script to run the ProductAPI independently for testing
# Includes comprehensive CMD curl examples for testing

param(
    [switch]$SkipBuild,
    [switch]$Help
)

if ($Help) {
    Write-Host "ProductAPI Standalone Launcher" -ForegroundColor Green
    Write-Host "Usage: ./StartStandalone.ps1 [-SkipBuild] [-Help]" -ForegroundColor Yellow
    Write-Host "  -SkipBuild  Skip the build step (faster startup)" -ForegroundColor White
    Write-Host "  -Help       Show this help message" -ForegroundColor White
    exit 0
}

# ========================================
# Header and Validation
# ========================================
Clear-Host
Write-Host "ProductAPI Standalone Launcher" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green

# Check if we're in the right directory
$currentPath = Get-Location
if (-not (Test-Path "Program.cs")) {
    Write-Host "ERROR: Not in ProductAPI directory!" -ForegroundColor Red
    Write-Host "Please run this script from: src/ProductAPI" -ForegroundColor Yellow
    Write-Host "Current location: $currentPath" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Quick fix:" -ForegroundColor Cyan
    Write-Host "  cd src/ProductAPI" -ForegroundColor White
    Write-Host "  ./StartStandalone.ps1" -ForegroundColor White
    exit 1
}

Write-Host "Current directory: $currentPath" -ForegroundColor Cyan
Write-Host ""

# ========================================
# Database Configuration Check  
# ========================================
Write-Host "Database Configuration Check" -ForegroundColor Yellow
Write-Host "============================" -ForegroundColor Gray

$connectionString = $null
$appsettingsPath = "appsettings.json"

if (Test-Path $appsettingsPath) {
    try {
        $appsettings = Get-Content $appsettingsPath | ConvertFrom-Json
        $connectionString = $appsettings.ConnectionStrings.DefaultConnection
        Write-Host "Found connection string in appsettings.json" -ForegroundColor Green
        Write-Host "  $connectionString" -ForegroundColor White
    } catch {
        Write-Host "Could not read appsettings.json" -ForegroundColor Yellow
    }
} else {
    Write-Host "appsettings.json not found" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Database Options:" -ForegroundColor Cyan
Write-Host "  Local PostgreSQL: Host=localhost;Port=5432;Database=productmanagement" -ForegroundColor White
Write-Host "  Docker PostgreSQL: docker run --name postgres-dev -e POSTGRES_DB=productmanagement -e POSTGRES_PASSWORD=Password123 -p 5432:5432 -d postgres:15-alpine" -ForegroundColor White
Write-Host ""

# ========================================
# Build Project
# ========================================
if (-not $SkipBuild) {
    Write-Host "Building Project..." -ForegroundColor Yellow
    Write-Host "===================" -ForegroundColor Gray
    
    Write-Host "Running: dotnet build --configuration Release" -ForegroundColor Gray
    dotnet build --configuration Release --verbosity quiet

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build failed!" -ForegroundColor Red
        Write-Host ""
        Write-Host "Try fixing common issues:" -ForegroundColor Yellow
        Write-Host "  dotnet restore" -ForegroundColor White
        Write-Host "  dotnet clean" -ForegroundColor White
        Write-Host "  Check for compilation errors above" -ForegroundColor White
        exit 1
    }

    Write-Host "Build successful!" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "Skipping build step..." -ForegroundColor Yellow
    Write-Host ""
}

# ========================================
# API Endpoints Information
# ========================================
Write-Host "API Server Information" -ForegroundColor Yellow
Write-Host "======================" -ForegroundColor Gray
Write-Host "The API will be available at:" -ForegroundColor Cyan
Write-Host "  HTTPS: https://localhost:7071" -ForegroundColor Green
Write-Host "  HTTP:  http://localhost:5262" -ForegroundColor Green
Write-Host ""

Write-Host "Available Endpoints:" -ForegroundColor Cyan
Write-Host "  GET    /products         - Get all products" -ForegroundColor White
Write-Host "  GET    /products/{id}    - Get specific product" -ForegroundColor White
Write-Host "  POST   /products         - Create new product" -ForegroundColor White
Write-Host "  PUT    /products/{id}    - Update existing product" -ForegroundColor White
Write-Host "  DELETE /products/{id}    - Delete product" -ForegroundColor White
Write-Host "  GET    /health           - Health check endpoint" -ForegroundColor White
Write-Host ""

# ========================================
# CMD Testing Examples
# ========================================
Write-Host "CMD Testing Commands" -ForegroundColor Yellow
Write-Host "====================" -ForegroundColor Gray
Write-Host "Copy and paste these commands into CMD to test the API:" -ForegroundColor Cyan
Write-Host ""

Write-Host "1. Health Check (Test if API is running)" -ForegroundColor Magenta
Write-Host "  curl http://localhost:5262/health" -ForegroundColor White
Write-Host ""

Write-Host "2. Get All Products" -ForegroundColor Magenta
Write-Host "  curl http://localhost:5262/products" -ForegroundColor White
Write-Host ""

Write-Host "3. Create a Product" -ForegroundColor Magenta
Write-Host "  curl -X POST http://localhost:5262/products ^" -ForegroundColor White
Write-Host "    -H `"Content-Type: application/json`" ^" -ForegroundColor White
Write-Host "    -d `"{\\`"name\\`":\\`"Sample Product\\`",\\`"price\\`":19.99,\\`"description\\`":\\`"A test product\\`"}`"" -ForegroundColor White
Write-Host ""

Write-Host "4. Get Product by ID (use ID from create response)" -ForegroundColor Magenta
Write-Host "  curl http://localhost:5262/products/1" -ForegroundColor White
Write-Host ""

Write-Host "5. Update a Product" -ForegroundColor Magenta
Write-Host "  curl -X PUT http://localhost:5262/products/1 ^" -ForegroundColor White
Write-Host "    -H `"Content-Type: application/json`" ^" -ForegroundColor White
Write-Host "    -d `"{\\`"id\\`":1,\\`"name\\`":\\`"Updated Product\\`",\\`"price\\`":29.99,\\`"description\\`":\\`"Updated description\\`"}`"" -ForegroundColor White
Write-Host ""

Write-Host "6. Delete a Product" -ForegroundColor Magenta
Write-Host "  curl -X DELETE http://localhost:5262/products/1" -ForegroundColor White
Write-Host ""

Write-Host "Pro Tips:" -ForegroundColor Cyan
Write-Host "  Use | jq for pretty JSON output (if jq is installed)" -ForegroundColor White
Write-Host "  Add -v flag to see full HTTP response details" -ForegroundColor White
Write-Host "  Use HTTP (port 5262) for simpler testing" -ForegroundColor White
Write-Host ""

# ========================================
# PowerShell Testing Examples (Bonus)
# ========================================
Write-Host "PowerShell Testing (Alternative to CMD)" -ForegroundColor Yellow
Write-Host "=======================================" -ForegroundColor Gray
Write-Host "PowerShell examples using Invoke-RestMethod:" -ForegroundColor Cyan
Write-Host ""

Write-Host "# Get all products" -ForegroundColor Gray
Write-Host "Invoke-RestMethod -Uri http://localhost:5262/products" -ForegroundColor White
Write-Host ""

Write-Host "# Create product" -ForegroundColor Gray
Write-Host "`$body = @{name=`"PS Product`"; price=25.50; description=`"PowerShell test`"} | ConvertTo-Json" -ForegroundColor White
Write-Host "Invoke-RestMethod -Uri http://localhost:5262/products -Method POST -Body `$body -ContentType `"application/json`"" -ForegroundColor White
Write-Host ""

# ========================================
# Start Server
# ========================================
Write-Host "Starting ProductAPI Server..." -ForegroundColor Yellow
Write-Host "=============================" -ForegroundColor Gray
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Cyan
Write-Host "Open a new terminal/cmd to run the test commands above" -ForegroundColor Cyan
Write-Host ""

Write-Host "Server starting..." -ForegroundColor Green
Write-Host ""

# Trap Ctrl+C for clean shutdown
trap {
    Write-Host ""
    Write-Host "Shutting down server..." -ForegroundColor Yellow
    Write-Host "Thanks for testing ProductAPI!" -ForegroundColor Green
    exit
}

# Start the API
try {
    dotnet run --no-build
} catch {
    Write-Host ""
    Write-Host "Server failed to start!" -ForegroundColor Red
    Write-Host "Check the error messages above for details." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "ProductAPI stopped. Thanks for testing!" -ForegroundColor Green 