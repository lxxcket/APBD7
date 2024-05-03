using System.Data.SqlClient;

namespace APBD7.Repository;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly string connectionString;

    public WarehouseRepository(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("MyConnectionString");
    }
    
    public int AddProductToWarehouse(OrderRequest orderRequest)
    {
        UpdatePurchaseOrder(orderRequest.IdProduct);
        return InsertProductWarehouse(orderRequest);
    }

    public int ProductExists(int id)
    {
        string query = "SELECT COUNT(1) FROM PRODUCT WHERE IdProduct = @id";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query,connection))
            {
                command.Parameters.AddWithValue("@id", id);
                return (int)command.ExecuteScalar();

            }
            
        }
    }

    public int WarehouseExists(int id)
    {
        string query = "SELECT COUNT(1) FROM Warehouse WHERE IdWarehouse = @id";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query,connection))
            {
                command.Parameters.AddWithValue("@id", id);
                return (int)command.ExecuteScalar();

            }
        }
    }
    public int PurchaseOrderExists(int productId, int amount, DateTime createdAt)
    {
        string query = "SELECT TOP 1 IdOrder FROM [Order] WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt < @CreatedAt ORDER BY CreatedAt DESC";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@CreatedAt", createdAt);
            
                object result = command.ExecuteScalar();
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
    public int OrderFulfilled(int idOrder)
    {
        string query = "SELECT COUNT(1) FROM Product_Warehouse WHERE IdOrder = @IdOrder";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@IdOrder", idOrder);
                return (int)command.ExecuteScalar();
            }
        }
    }
    private void UpdatePurchaseOrder(int productId)
    {
        string query = "UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdProduct = @ProductId";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProductId", productId);
                command.ExecuteNonQuery();
            }
        }
    }
    private int InsertProductWarehouse(OrderRequest request)
    {
        string query = @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) 
                             VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, GETDATE());
                             SELECT SCOPE_IDENTITY();";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
                command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
                command.Parameters.AddWithValue("@IdOrder",
                    GetOrderId(request.IdProduct, request.Amount, request.CreatedAt));
                command.Parameters.AddWithValue("@Amount", request.Amount);
                command.Parameters.AddWithValue("@Price",
                    GetProductPrice(request.IdProduct) * request.Amount);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }

    private double GetProductPrice(int requestIdProduct)
    {
        string query = "SELECT Price FROM Product WHERE IdProduct = @ProductId";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProductId", requestIdProduct);
                return Convert.ToDouble(command.ExecuteScalar());
            }
        }
    }

    private int GetOrderId(int requestIdProduct, int requestAmount, DateTime requestCreatedAt)
    {
        string query = "SELECT TOP 1 IdOrder FROM [Order] WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt < @CreatedAt ORDER BY CreatedAt DESC";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProductId", requestIdProduct);
                command.Parameters.AddWithValue("@Amount", requestAmount);
                command.Parameters.AddWithValue("@CreatedAt", requestCreatedAt);
                return (int)command.ExecuteScalar();
            }
        }
    }
}