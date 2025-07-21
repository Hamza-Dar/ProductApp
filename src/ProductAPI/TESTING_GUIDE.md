# ProductAPI Testing Guide

## 🚀 Quick Start (2 Steps)

### Step 1: Start the API Server
```powershell
cd src/ProductAPI
./StartStandalone.ps1
```

### Step 2: Test the API (Choose One)

**Option A: Automated Testing (Recommended)**
```cmd
# In a new CMD window
cd src/ProductAPI
TestAPI.cmd
```

**Option B: Manual Testing**
Copy commands from the server output or use the examples below.

---

## 📋 Available Scripts

| Script | Purpose | Usage |
|--------|---------|-------|
| `StartStandalone.ps1` | Start API server with testing instructions | `./StartStandalone.ps1` |
| `TestAPI.cmd` | Automated CMD test suite | `TestAPI.cmd` |

### StartStandalone.ps1 Options

```powershell
./StartStandalone.ps1              # Normal startup with build
./StartStandalone.ps1 -SkipBuild   # Skip build step (faster)
./StartStandalone.ps1 -Help        # Show help
```

---

## 🧪 Manual Testing Commands

### Health Check
```cmd
curl http://localhost:5262/health
```

### Get All Products
```cmd
curl http://localhost:5262/products
```

### Create a Product
```cmd
curl -X POST http://localhost:5262/products ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"My Product\",\"price\":25.99,\"description\":\"Test product\"}"
```

### Get Product by ID
```cmd
curl http://localhost:5262/products/1
```

### Update Product
```cmd
curl -X PUT http://localhost:5262/products/1 ^
  -H "Content-Type: application/json" ^
  -d "{\"id\":1,\"name\":\"Updated Product\",\"price\":35.99,\"description\":\"Updated\"}"
```

### Delete Product
```cmd
curl -X DELETE http://localhost:5262/products/1
```

---

## 💡 PowerShell Alternative

If you prefer PowerShell over CMD:

```powershell
# Get all products
Invoke-RestMethod -Uri http://localhost:5262/products

# Create product
$product = @{name="PS Product"; price=19.99; description="PowerShell test"} | ConvertTo-Json
Invoke-RestMethod -Uri http://localhost:5262/products -Method POST -Body $product -ContentType "application/json"

# Update product
$updated = @{id=1; name="Updated PS Product"; price=29.99; description="Updated via PowerShell"} | ConvertTo-Json
Invoke-RestMethod -Uri http://localhost:5262/products/1 -Method PUT -Body $updated -ContentType "application/json"

# Delete product
Invoke-RestMethod -Uri http://localhost:5262/products/1 -Method DELETE
```

---

## 🔧 Troubleshooting

### API Won't Start
1. **Check database connection** - Make sure PostgreSQL is running
2. **Run build manually** - `dotnet build`
3. **Check ports** - Make sure 5262 and 7071 are available
4. **Try without build** - `./StartStandalone.ps1 -SkipBuild`

### Curl Commands Not Working
1. **Check API is running** - `curl http://localhost:5262/health`
2. **Use HTTP instead of HTTPS** - Use port 5262 instead of 7071
3. **Verify JSON formatting** - Check quote escaping: `{\"name\":\"value\"}`
4. **Try the automated test** - Run `TestAPI.cmd` instead

### Database Issues
```bash
# Quick PostgreSQL Docker setup
docker run --name postgres-dev ^
  -e POSTGRES_DB=productmanagement ^
  -e POSTGRES_PASSWORD=Password123 ^
  -p 5432:5432 -d postgres:15-alpine
```

### Common Errors

| Error | Solution |
|-------|----------|
| `Could not resolve host` | Check API is running, use correct port |
| `Connection refused` | API not started or wrong port |
| `JSON parse error` | Check quote escaping in curl commands |
| `Database connection failed` | Check PostgreSQL is running |

---

## 📊 Expected Responses

### Successful Product Creation
```json
{
  "id": 1,
  "name": "My Product",
  "price": 25.99,
  "description": "Test product"
}
```

### Get All Products
```json
[
  {
    "id": 1,
    "name": "My Product",
    "price": 25.99,
    "description": "Test product"
  }
]
```

### Health Check
```
Healthy
```

---

## 🎯 Testing Workflow

1. **Start API** - `./StartStandalone.ps1`
2. **Verify Health** - `curl http://localhost:5262/health`
3. **Test CRUD Operations**:
   - Create → Read → Update → Delete
4. **Use automated test** - `TestAPI.cmd` for full suite
5. Alternatively, Test manually via ProductAPI.http
**Happy Testing! 🚀** 