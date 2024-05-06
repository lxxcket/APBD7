using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace APBD7.Repository
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly string connectionString;

        public WarehouseRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("MyConnectionString");
        }

        public async Task<int> AddProductToWarehouseAsync(OrderRequest orderRequest)
        {
            await UpdatePurchaseOrderAsync(orderRequest.IdProduct);
            return await InsertProductWarehouseAsync(orderRequest);
        }

        public async Task<int> ProductExistsAsync(int id)
        {
            string query = "SELECT COUNT(1) FROM PRODUCT WHERE IdProduct = @id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    return (int)await command.ExecuteScalarAsync();
                }
            }
        }

        public async Task<int> WarehouseExistsAsync(int id)
        {
            string query = "SELECT COUNT(1) FROM Warehouse WHERE IdWarehouse = @id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    return (int)await command.ExecuteScalarAsync();
                }
            }
        }

        public async Task<int> PurchaseOrderExistsAsync(int productId, int amount, DateTime createdAt)
        {
            string query = "SELECT TOP 1 IdOrder FROM [Order] WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt < @CreatedAt ORDER BY CreatedAt DESC";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@CreatedAt", createdAt);

                    object result = await command.ExecuteScalarAsync();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        throw new Exception("Purchase order not found.");
                    }
                }
            }
        }

        public async Task<int> OrderFulfilledAsync(int idOrder)
        {
            string query = "SELECT COUNT(1) FROM Product_Warehouse WHERE IdOrder = @IdOrder";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdOrder", idOrder);
                    return (int)await command.ExecuteScalarAsync();
                }
            }
        }

        public async Task<int> AddProductProcedureAsync(OrderRequest orderRequest)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("AddProductToWarehouse", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProduct", orderRequest.IdProduct);
                    command.Parameters.AddWithValue("@IdWarehouse", orderRequest.IdWarehouse);
                    command.Parameters.AddWithValue("@Amount", orderRequest.Amount);
                    command.Parameters.AddWithValue("@CreatedAt", orderRequest.CreatedAt);

                    return Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
        }

        private async Task UpdatePurchaseOrderAsync(int productId)
        {
            string query = "UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdProduct = @ProductId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task<int> InsertProductWarehouseAsync(OrderRequest request)
        {
            string query = @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) 
                             VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, GETDATE());
                             SELECT SCOPE_IDENTITY();";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
                    command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
                    command.Parameters.AddWithValue("@IdOrder",
                        await GetOrderIdAsync(request.IdProduct, request.Amount, request.CreatedAt));
                    command.Parameters.AddWithValue("@Amount", request.Amount);
                    command.Parameters.AddWithValue("@Price",
                         await GetProductPriceAsync(request.IdProduct) * request.Amount);
 
                    return Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
        }

        private async Task<double> GetProductPriceAsync(int requestIdProduct)
        {
            string query = "SELECT Price FROM Product WHERE IdProduct = @ProductId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", requestIdProduct);
                    return Convert.ToDouble(await command.ExecuteScalarAsync());
                }
            }
        }

        private async Task<int> GetOrderIdAsync(int requestIdProduct, int requestAmount, DateTime requestCreatedAt)
        {
            string query = "SELECT TOP 1 IdOrder FROM [Order] WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt < @CreatedAt ORDER BY CreatedAt DESC";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", requestIdProduct);
                    command.Parameters.AddWithValue("@Amount", requestAmount);
                    command.Parameters.AddWithValue("@CreatedAt", requestCreatedAt);
                    return (int)await command.ExecuteScalarAsync();
                }
            }
        }
    }
}
