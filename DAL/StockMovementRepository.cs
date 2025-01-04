using SalesManagementWebApp.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

public class StockMovementRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public StockMovementRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IEnumerable<StockMovementViewModel> GetAllStockMovements()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var stockMovements = new List<StockMovementViewModel>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT sm.*, p.Name AS ProductName FROM dbo.StockMovement sm LEFT JOIN Product p ON sm.ProductID = p.ID  WHERE sm.isDeleted = 0";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    stockMovements.Add(new StockMovementViewModel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ID")),
                        ProductId = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                        MovementType = reader.GetString(reader.GetOrdinal("MovementType")),
                        Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note"))
                    });
                }
            }
        }
        return stockMovements;
    }

    public void AddStockMovement(StockMovementViewModel model)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.CreateStockMovement";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@ProductID", model.ProductId));
        command.Parameters.Add(new SqlParameter("@MovementType", model.MovementType));
        command.Parameters.Add(new SqlParameter("@Date", model.Date));
        command.Parameters.Add(new SqlParameter("@Quantity", model.Quantity));
        command.Parameters.Add(new SqlParameter("@Note", model.Note ?? (object)DBNull.Value));
        var newStockMovementId = new SqlParameter("@NewStockMovementID", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(newStockMovementId);

        command.ExecuteNonQuery();
    }

    public StockMovementViewModel GetStockMovementById(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT sm.*, p.Name AS ProductName FROM dbo.StockMovement sm LEFT JOIN Product p ON sm.ProductID = p.ID  WHERE sm.isDeleted = 0 AND sm.ID = @StockMovementID";
        command.Parameters.Add(new SqlParameter("@StockMovementID", id));

        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                return new StockMovementViewModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductID")),
                    ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                    MovementType = reader.GetString(reader.GetOrdinal("MovementType")),
                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note"))
                };
            }
        }
        return null;
    }

    public void UpdateStockMovement(StockMovementViewModel model)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.UpdateStockMovement";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@StockMovementID", model.Id));
        command.Parameters.Add(new SqlParameter("@ProductID", model.ProductId));
        command.Parameters.Add(new SqlParameter("@MovementType", model.MovementType));
        command.Parameters.Add(new SqlParameter("@Date", model.Date));
        command.Parameters.Add(new SqlParameter("@Quantity", model.Quantity));
        command.Parameters.Add(new SqlParameter("@Note", model.Note ?? (object)DBNull.Value));

        command.ExecuteNonQuery();
    }

    public void DeleteStockMovement(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.DeleteStockMovement";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@StockMovementID", id));

        command.ExecuteNonQuery();
    }
}
