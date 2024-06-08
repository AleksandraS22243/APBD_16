using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly string _connectionString;

        public WarehouseController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("WarehouseDatabase")
                              ?? throw new ArgumentNullException(nameof(configuration), "Connection string is missing.");
        }

        [HttpPost("add-product")]
        public async Task<IActionResult> AddProductToWarehouse([FromBody] AddProductRequest request)
        {
            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than 0.");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Check if Product exists
                var productExists = await CheckIfExistsAsync(connection, "SELECT COUNT(1) FROM Product WHERE IdProduct = @IdProduct", new SqlParameter("@IdProduct", request.IdProduct));
                if (!productExists)
                    return NotFound("Product not found.");

                // Check if Warehouse exists
                var warehouseExists = await CheckIfExistsAsync(connection, "SELECT COUNT(1) FROM Warehouse WHERE IdWarehouse = @IdWarehouse", new SqlParameter("@IdWarehouse", request.IdWarehouse));
                if (!warehouseExists)
                    return NotFound("Warehouse not found.");

                // Check if there's a matching Order
                var orderId = await GetOrderIdAsync(connection, request.IdProduct, request.Amount, request.CreatedAt);
                if (orderId == null)
                    return BadRequest("No matching order found or order already fulfilled.");

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update Order
                        var updateOrderCommand = new SqlCommand("UPDATE [Order] SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder", connection, transaction);
                        updateOrderCommand.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);
                        updateOrderCommand.Parameters.AddWithValue("@IdOrder", orderId);
                        await updateOrderCommand.ExecuteNonQueryAsync();

                        // Insert into Product_Warehouse
                        var price = await GetProductPriceAsync(connection, request.IdProduct);
                        var insertCommand = new SqlCommand("INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt); SELECT SCOPE_IDENTITY();", connection, transaction);
                        insertCommand.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
                        insertCommand.Parameters.AddWithValue("@IdProduct", request.IdProduct);
                        insertCommand.Parameters.AddWithValue("@IdOrder", orderId);
                        insertCommand.Parameters.AddWithValue("@Amount", request.Amount);
                        insertCommand.Parameters.AddWithValue("@Price", request.Amount * price);
                        insertCommand.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        var newId = await insertCommand.ExecuteScalarAsync();
                        transaction.Commit();

                        return Ok(new { NewId = newId });
                    }
                    catch
                    {
                        transaction.Rollback();
                        return StatusCode(500, "Internal server error");
                    }
                }
            }
        }

        [HttpPost("add-product-stored-procedure")]
        public async Task<IActionResult> AddProductToWarehouseUsingProc([FromBody] AddProductRequest request)
        {
            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than 0.");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("AddProductToWarehouse", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
                    command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
                    command.Parameters.AddWithValue("@Amount", request.Amount);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    try
                    {
                        var newId = await command.ExecuteScalarAsync();
                        return Ok(new { NewId = newId });
                    }
                    catch (SqlException ex)
                    {
                        return StatusCode(500, ex.Message);
                    }
                }
            }
        }

        private async Task<bool> CheckIfExistsAsync(SqlConnection connection, string query, SqlParameter parameter)
        {
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.Add(parameter);
                return (int)await command.ExecuteScalarAsync() > 0;
            }
        }

        private async Task<decimal> GetProductPriceAsync(SqlConnection connection, int productId)
        {
            using (var command = new SqlCommand("SELECT Price FROM Product WHERE IdProduct = @IdProduct", connection))
            {
                command.Parameters.AddWithValue("@IdProduct", productId);
                return (decimal)await command.ExecuteScalarAsync();
            }
        }

        private async Task<int?> GetOrderIdAsync(SqlConnection connection, int productId, int amount, DateTime createdAt)
        {
            using (var command = new SqlCommand("SELECT TOP 1 IdOrder FROM [Order] o LEFT JOIN Product_Warehouse pw ON o.IdOrder = pw.IdOrder WHERE o.IdProduct = @IdProduct AND o.Amount = @Amount AND pw.IdProductWarehouse IS NULL AND o.CreatedAt < @CreatedAt", connection))
            {
                command.Parameters.AddWithValue("@IdProduct", productId);
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@CreatedAt", createdAt);

                return (int?)await command.ExecuteScalarAsync();
            }
        }
    }
}
