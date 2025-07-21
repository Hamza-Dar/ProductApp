@echo off
REM ========================================
REM ProductAPI Test Commands (CMD)
REM ========================================
REM Make sure ProductAPI is running first!
REM Run: ./StartStandalone.ps1

echo.
echo ========================================
echo ProductAPI Test Suite (CMD)
echo ========================================
echo.
echo Make sure the API is running first:
echo   cd src/ProductAPI
echo   ./StartStandalone.ps1
echo.

set API_URL=http://localhost:5262

echo 1. Testing Health Check...
echo Command: curl %API_URL%/health
curl %API_URL%/health
echo.
echo.

echo 2. Testing Get All Products (should be empty initially)...
echo Command: curl %API_URL%/products
curl %API_URL%/products
echo.
echo.

echo 3. Creating a Product...
echo Command: curl -X POST %API_URL%/products ^
echo   -H "Content-Type: application/json" ^
echo   -d "{\"name\":\"Test Product\",\"price\":19.99,\"description\":\"A test product\"}"
curl -X POST %API_URL%/products ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Test Product\",\"price\":19.99,\"description\":\"A test product\"}"
echo.
echo.

echo 4. Getting All Products Again (should show the created product)...
curl %API_URL%/products
echo.
echo.

echo 5. Getting Product by ID 1...
echo Command: curl %API_URL%/products/1
curl %API_URL%/products/1
echo.
echo.

echo 6. Updating Product ID 1...
echo Command: curl -X PUT %API_URL%/products/1 ^
echo   -H "Content-Type: application/json" ^
echo   -d "{\"id\":1,\"name\":\"Updated Test Product\",\"price\":29.99,\"description\":\"Updated description\"}"
curl -X PUT %API_URL%/products/1 ^
  -H "Content-Type: application/json" ^
  -d "{\"id\":1,\"name\":\"Updated Test Product\",\"price\":29.99,\"description\":\"Updated description\"}"
echo.
echo.

echo 7. Getting Updated Product...
curl %API_URL%/products/1
echo.
echo.

echo 8. Deleting Product ID 1...
echo Command: curl -X DELETE %API_URL%/products/1
curl -X DELETE %API_URL%/products/1
echo.
echo.

echo 9. Confirming Deletion (should be empty or not found)...
curl %API_URL%/products
echo.
echo.

echo ========================================
echo Test Complete!
echo ========================================
echo.
echo If you see successful responses above, your API is working correctly!
echo.
pause 