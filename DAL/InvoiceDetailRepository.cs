using Microsoft.Data.SqlClient;
using SalesManagementWebApp.Models;
using System.Data;

public class InvoiceDetailRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public InvoiceDetailRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public InvoiceDetailViewModel GetInvoiceDetailById(int detailId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "select i.*, p.Name from InvoiceDetail i left join Product p ON i.ProductID = p.ID WHERE i.ID = @DetailID";
        command.Parameters.Add(new SqlParameter("@DetailID", detailId));

        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                return new InvoiceDetailViewModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    InvoiceID = reader.GetInt32(reader.GetOrdinal("InvoiceID")),
                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                    ProductName = reader.GetString(reader.GetOrdinal("Name")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                    TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice"))
                };
            }
        }

        return null;
    }

    public IEnumerable<InvoiceDetailViewModel> GetInvoiceDetailsByInvoiceId(int invoiceId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var details = new List<InvoiceDetailViewModel>();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT i.*, (i.Quantity * i.UnitPrice) AS TotalPrice, p.Name FROM dbo.InvoiceDetail i LEFT JOIN Product p ON i.ProductID = p.ID WHERE i.InvoiceID = @InvoiceID AND i.isDeleted = 0;";
        command.Parameters.Add(new SqlParameter("@InvoiceID", invoiceId));

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                details.Add(new InvoiceDetailViewModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    InvoiceID = reader.GetInt32(reader.GetOrdinal("InvoiceID")),
                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                    ProductName = reader.GetString(reader.GetOrdinal("Name")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                    TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice"))
                });
            }
        }

        return details;
    }

    public int AddInvoiceDetail(InvoiceDetailViewModel detail)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.CreateInvoiceDetail";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@InvoiceID", detail.InvoiceID));
        command.Parameters.Add(new SqlParameter("@ProductID", detail.ProductID));
        command.Parameters.Add(new SqlParameter("@Quantity", detail.Quantity));
        command.Parameters.Add(new SqlParameter("@UnitPrice", detail.UnitPrice));

        var outputParam = new SqlParameter("@NewInvoiceDetailID", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputParam);

        command.ExecuteNonQuery();

        return (int)outputParam.Value;
    }

    public void UpdateInvoiceDetail(InvoiceDetailViewModel detail)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.UpdateInvoiceDetail";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@InvoiceDetailID", detail.Id));
        command.Parameters.Add(new SqlParameter("@InvoiceID", detail.InvoiceID));
        command.Parameters.Add(new SqlParameter("@ProductID", detail.ProductID));
        command.Parameters.Add(new SqlParameter("@Quantity", detail.Quantity));
        command.Parameters.Add(new SqlParameter("@UnitPrice", detail.UnitPrice));

        command.ExecuteNonQuery();
    }

    public void DeleteInvoiceDetail(int detailId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.DeleteInvoiceDetail";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@InvoiceDetailID", detailId));

        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqlException ex) when (ex.Number == 50013)
        {
            throw new InvalidOperationException("Fatura detayı bulunamadı veya silinemedi.", ex);
        }
    }

}
