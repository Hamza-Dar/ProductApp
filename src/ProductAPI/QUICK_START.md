# ProductAPI - Quick Start Guide

## 🚀 Run Standalone (3 Steps)

### 1. Setup Database
**Option A**: Local PostgreSQL
```sql
CREATE DATABASE productmanagement;
```

**Option B**: Docker PostgreSQL
```bash
docker run --name postgres-standalone -e POSTGRES_DB=productmanagement -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=Password123 -p 5432:5432 -d postgres:15-alpine
```

### 2. Start API
```bash
cd src/ProductAPI
dotnet run
```
**OR use the helper script:**
```bash
cd src/ProductAPI
./StartStandalone.ps1
```

### 3. Test Endpoints
**Base URLs:**
- HTTPS: `https://localhost:7071`
- HTTP: `http://localhost:5262`

**Quick Test (PowerShell/Bash/CMD):**
```bash
curl https://localhost:7071/products
```
or
```bash
curl http://localhost:5262/products
```

**Quick Test with Data Creation:**
```bash
# PowerShell/Bash
curl -X POST https://localhost:7071/products \
  -H "Content-Type: application/json" \
  -d '{"name": "Quick Test", "price": 19.99, "description": "Test product"}'
```
```cmd
# CMD
curl -X POST http://localhost:5262/products ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Quick Test\",\"price\":19.99,\"description\":\"Test product\"}"
```
## 📋 Available Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/products` | Get all products |
| GET | `/products/{id}` | Get specific product |
| POST | `/products` | Create new product |
| PUT | `/products/{id}` | Update product |
| DELETE | `/products/{id}` | Delete product |

## 🧪 Sample Requests

### Create Product
**PowerShell/Bash:**
```bash
curl -X POST https://localhost:7071/products \
  -H "Content-Type: application/json" \
  -d '{"name": "Test Product", "price": 29.99, "description": "Sample product"}'
```

**CMD:**
```cmd
curl -X POST http://localhost:5262/products ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Test Product\",\"price\":29.99,\"description\":\"Sample product\"}"
```

### Get All Products
```bash
curl https://localhost:7071/products
```
or
```bash
curl http://localhost:5262/products
```

### Update Product
**PowerShell/Bash:**
```bash
curl -X PUT https://localhost:7071/products/1 \
  -H "Content-Type: application/json" \
  -d '{"name": "Updated Product", "price": 39.99, "description": "Updated description"}'
```

**CMD:**
```cmd
curl -X PUT http://localhost:5262/products/1 ^
  -H "Content-Type: application/json" ^
  -d "{\"id\":1,\"name\":\"Updated Product\",\"price\":39.99,\"description\":\"Updated description\"}"
```

### Delete Product
**PowerShell/Bash:**
```bash
curl -X DELETE https://localhost:7071/products/1
```

**CMD:**
```cmd
curl -X DELETE http://localhost:5262/products/1
```

### Get Product by ID
**PowerShell/Bash/CMD:**
```bash
curl https://localhost:7071/products/1
```
or
```bash
curl http://localhost:5262/products/1
```

## 💡 Command Format Notes

**Key Differences for CMD:**
- Line continuation: Use `^` instead of `\`
- Quote escaping: Use `\"` instead of `"` inside JSON strings
- Keep regular quotes around headers: `"Content-Type: application/json"`
- Escape JSON quotes with backslash: `"{\"name\":\"value\"}"`
## 📄 Testing Files

- **`ProductAPI.http`** - Comprehensive HTTP test file for VS Code/REST clients
- **`STANDALONE_GUIDE.md`** - Detailed setup and troubleshooting guide
- **`StartStandalone.ps1`** - PowerShell script to launch the API

## ✅ Success Indicators

When running correctly, you'll see:
```
info: Program[0]
      Database schema initialized successfully
Now listening on: https://localhost:7071
Now listening on: http://localhost:5262
```
