using Dapper;
using Npgsql;
using ProductAPI.Models;
using Microsoft.Extensions.Logging;

namespace ProductAPI.Data
{
    public class ProductRepository
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(NpgsqlDataSource dataSource, ILogger<ProductRepository> logger)
        {
            _dataSource = dataSource;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            const string sql = @"
                SELECT 
                    id as Id, 
                    name as Name, 
                    price as Price, 
                    description as Description
                FROM products 
                ORDER BY id";
            
            var allProducts = await connection.QueryAsync<Product>(sql);
            return allProducts;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            const string sql = @"
                SELECT 
                    id as Id, 
                    name as Name, 
                    price as Price, 
                    description as Description
                FROM products 
                WHERE id = @Id";
            
            var product = await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
            return product;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            const string sql = @"
                INSERT INTO products (name, price, description) 
                VALUES (@Name, @Price, @Description) 
                RETURNING 
                    id as Id, 
                    name as Name, 
                    price as Price, 
                    description as Description";
            
            var createdProduct = await connection.QuerySingleAsync<Product>(sql, product);
            return createdProduct;
        }

        public async Task<Product?> UpdateAsync(int id, Product product)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            const string sql = @"
                UPDATE products 
                SET name = @Name, price = @Price, description = @Description 
                WHERE id = @Id 
                RETURNING 
                    id as Id, 
                    name as Name, 
                    price as Price, 
                    description as Description";
            
            var updatedProduct = await connection.QueryFirstOrDefaultAsync<Product>(sql, 
                new { Id = id, product.Name, product.Price, product.Description });
            return updatedProduct;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            var rowsAffected = await connection.ExecuteAsync("DELETE FROM products WHERE id = @Id", new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            var exists = await connection.QueryFirstOrDefaultAsync<bool>(
                "SELECT EXISTS(SELECT 1 FROM products WHERE id = @Id)", new { Id = id });
            return exists;
        }

        public async Task EnsureDatabaseCreatedAsync()
        {
            try
            {
                _logger.LogInformation("Attempting to connect to database and create schema...");
                _logger.LogInformation("Connection string: {ConnectionString}", _dataSource.ConnectionString);
                
                using var connection = await _dataSource.OpenConnectionAsync();
                _logger.LogInformation("Successfully connected to database");
                
                const string sql = @"
                    CREATE TABLE IF NOT EXISTS products (
                        id SERIAL PRIMARY KEY,
                        name VARCHAR(100) NOT NULL,
                        price NUMERIC(10,2) NOT NULL CHECK (price > 0),
                        description TEXT DEFAULT ''
                    );

                    CREATE INDEX IF NOT EXISTS idx_products_name ON products(name);";
                
                await connection.ExecuteAsync(sql);
                _logger.LogInformation("Database schema created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create database schema. Connection string: {ConnectionString}", _dataSource.ConnectionString);
                throw;
            }
        }
    }
}
