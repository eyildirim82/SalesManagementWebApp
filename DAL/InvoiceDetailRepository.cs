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

    public IEnumerable<InvoiceDetailViewModel> GetInvoiceDetailsByInvoiceId(int invoiceId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var invoiceDetails = new List<InvoiceDetailViewModel>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
            SELECT 
                d.ID, 
                d.InvoiceID, 
                p.Name AS ProductName, 
                d.Quantity, 
                d.UnitPrice, 
                (d.Quantity * d.UnitPrice) AS LineTotal
            FROM dbo.InvoiceDetail d
            INNER JOIN dbo.Product p ON d.ProductID = p.ID
            WHERE d.InvoiceID = @InvoiceID AND d.isDeleted = 0";
            command.Parameters.Add(new SqlParameter("@InvoiceID", invoiceId));

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    invoiceDetails.Add(new InvoiceDetailViewModel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ID")),
                        InvoiceID = reader.GetInt32(reader.GetOrdinal("InvoiceID")),
                        ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                        TotalPrice = reader.GetDecimal(reader.GetOrdinal("LineTotal"))
                    });
                }
            }
        }

        return invoiceDetails;
    }

}
