using Microsoft.Data.SqlClient;
using SalesManagementWebApp.Models;
using System.Data;

public class InvoiceRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public InvoiceRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IEnumerable<InvoiceViewModel> GetAllInvoices()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var invoices = new List<InvoiceViewModel>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT i.*, c.Name CustomerName FROM dbo.Invoice i LEFT JOIN Customer c ON i.CustomerID = c.ID WHERE i.isDeleted = 0 ORDER BY Status DESC, Type";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    invoices.Add(new InvoiceViewModel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ID")),
                        CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
                        Type = reader.GetString(reader.GetOrdinal("Type")),
                        Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                        DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? null : reader.GetDateTime(reader.GetOrdinal("DueDate")),
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        ExchangeRate = reader.GetDecimal(reader.GetOrdinal("ExchangeRate")),
                        DiscountRate = reader.GetDecimal(reader.GetOrdinal("DiscountRate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                        IsDeleted = reader.GetBoolean(reader.GetOrdinal("isDeleted"))
                    });
                }
            }
        }

        return invoices;
    }

    public int AddInvoice(InvoiceViewModel model)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.CreateInvoice";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@CustomerID", model.CustomerId));
        command.Parameters.Add(new SqlParameter("@Type", model.Type));
        command.Parameters.Add(new SqlParameter("@Date", model.Date));
        command.Parameters.Add(new SqlParameter("@DueDate", model.DueDate ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Status", model.Status));
        command.Parameters.Add(new SqlParameter("@ExchangeRate", model.ExchangeRate));
        command.Parameters.Add(new SqlParameter("@DiscountRate", model.DiscountRate));
        command.Parameters.Add(new SqlParameter("@Notes", model.Notes ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@TotalAmount", model.TotalAmount));

        var outputParam = new SqlParameter("@NewInvoiceID", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(outputParam);

        command.ExecuteNonQuery();
        return (int)outputParam.Value; // Fix for CS0029 and CS1002
    }

    public InvoiceViewModel GetInvoiceById(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM dbo.Invoice WHERE ID = @InvoiceID AND isDeleted = 0";
        command.Parameters.Add(new SqlParameter("@InvoiceID", id));

        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                return new InvoiceViewModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                    Type = reader.GetString(reader.GetOrdinal("Type")),
                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                    DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? null : reader.GetDateTime(reader.GetOrdinal("DueDate")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    ExchangeRate = reader.GetDecimal(reader.GetOrdinal("ExchangeRate")),
                    DiscountRate = reader.GetDecimal(reader.GetOrdinal("DiscountRate")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    IsDeleted = reader.GetBoolean(reader.GetOrdinal("isDeleted"))
                };
            }
        }
        return null;
    }

    public void UpdateInvoice(InvoiceViewModel model)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.UpdateInvoice";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@InvoiceID", model.Id));
        command.Parameters.Add(new SqlParameter("@CustomerID", model.CustomerId));
        command.Parameters.Add(new SqlParameter("@Type", model.Type ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Date", model.Date));
        command.Parameters.Add(new SqlParameter("@DueDate", model.DueDate ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Status", model.Status ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@ExchangeRate", model.ExchangeRate));
        command.Parameters.Add(new SqlParameter("@DiscountRate", model.DiscountRate));
        command.Parameters.Add(new SqlParameter("@Notes", model.Notes ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@TotalAmount", model.TotalAmount));

        command.ExecuteNonQuery();
    }

    public void DeleteInvoice(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.DeleteInvoice";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@InvoiceID", id));

        command.ExecuteNonQuery();
    }
}
