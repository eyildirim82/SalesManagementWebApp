using SalesManagementWebApp.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

public class PaymentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PaymentRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IEnumerable<PaymentViewModel> GetAllPayments()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var payments = new List<PaymentViewModel>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM dbo.Payment WHERE isDeleted = 0";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    payments.Add(new PaymentViewModel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ID")),
                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                        PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate")),
                        PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes"))
                    });
                }
            }
        }

        return payments;
    }

    public void AddPayment(PaymentViewModel model)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.AddPayment";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@InvoiceID", model.InvoiceId));
        command.Parameters.Add(new SqlParameter("@Amount", model.Amount));
        command.Parameters.Add(new SqlParameter("@PaymentDate", model.PaymentDate));
        command.Parameters.Add(new SqlParameter("@PaymentMethod", model.PaymentMethod));
        command.Parameters.Add(new SqlParameter("@Notes", model.Notes ?? (object)DBNull.Value));
        var newPaymentIdParam = new SqlParameter("@NewPaymentID", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(newPaymentIdParam);

        command.ExecuteNonQuery();

        var newPaymentId = (int)newPaymentIdParam.Value;
    }

    public PaymentViewModel GetPaymentById(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM dbo.Payment WHERE ID = @PaymentID AND isDeleted = 0";
        command.Parameters.Add(new SqlParameter("@PaymentID", id));

        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                return new PaymentViewModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                    Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                    PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate")),
                    PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes"))
                };
            }
        }

        return null;
    }

    public void UpdatePayment(PaymentViewModel model)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.UpdatePayment";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@PaymentID", model.Id));
        command.Parameters.Add(new SqlParameter("@InvoiceID", model.InvoiceId));
        command.Parameters.Add(new SqlParameter("@Amount", model.Amount));
        command.Parameters.Add(new SqlParameter("@PaymentDate", model.PaymentDate));
        command.Parameters.Add(new SqlParameter("@PaymentMethod", model.PaymentMethod));
        command.Parameters.Add(new SqlParameter("@Notes", model.Notes ?? (object)DBNull.Value));

        command.ExecuteNonQuery();
    }

    public void DeletePayment(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.DeletePayment";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@PaymentID", id));

        command.ExecuteNonQuery();
    }
}
