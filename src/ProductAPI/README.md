# ProductAPI

A ASP.NET Core Web API for product management built with FastEndpoints and best practices.

## Architecture

### Project Structure
```
ProductAPI/
├── Data/
│   └── ProductRepository.cs          # Data access layer
├── DTOs/
│   └── ProductDTOs.cs               # Data Transfer Objects
├── Endpoints/
│   └── Products/                    # Feature-based endpoint organization
│       ├── CreateProductEndpoint.cs
│       ├── DeleteProductEndpoint.cs
│       ├── GetAllProductsEndpoint.cs
│       ├── GetProductByIdEndpoint.cs
│       └── UpdateProductEndpoint.cs
├── Extensions/
│   └── ProductMappingExtensions.cs  # Entity/DTO mapping extensions
├── Models/
│   └── Product.cs                   # Domain entities
└── Program.cs                       # Application entry point
```

### Key Features

- **Separation of Concerns**: Each endpoint is in its own file with clear responsibilities
- **Enterprise Patterns**: Proper DTOs, mapping extensions, and structured validation
- **Comprehensive Logging**: Structured logging throughout all operations
- **Error Handling**: Proper exception handling with meaningful error responses
- **Data Validation**: Both attribute-based and custom business validation
- **Repository Pattern**: Clean data access abstraction
- **HTTP Standards**: Proper status codes and RESTful conventions

## API Endpoints

| Method | Endpoint | Description | Status Codes |
|--------|----------|-------------|--------------|
| GET | `/products` | Retrieve all products | 200, 500 |
| GET | `/products/{id}` | Get product by ID | 200, 404, 500 |
| POST | `/products` | Create new product | 201, 400, 500 |
| PUT | `/products/{id}` | Update existing product | 200, 400, 404, 500 |
| DELETE | `/products/{id}` | Delete product | 204, 404, 500 |

## Business Rules

### Product Validation
- **Name**: Required, 1-100 characters
- **Price**: Required, must be greater than 0
- **Description**: Optional, max 1000 characters

### Error Handling
- Validation errors return 400 Bad Request with detailed field-level errors
- Not found resources return 404 Not Found
- Server errors return 500 Internal Server Error with logged details

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **API Framework**: FastEndpoints
- **Database**: PostgreSQL
- **ORM**: Dapper
- **Logging**: Microsoft.Extensions.Logging

## Development

### Testing
Use the `ProductAPI.http` file for endpoint testing with various scenarios including:
- All CRUD operations
- Validation error cases
- Not found scenarios
- Edge cases

OR Use **Use automated test** - `TestAPI.cmd` for full suite

### Database
The application automatically creates the required `products` table on startup.