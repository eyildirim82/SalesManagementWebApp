using Microsoft.Data.SqlClient;
using SalesManagementWebApp.Models;
using System.Data;

public class CustomerRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CustomerRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IEnumerable<CustomerViewModel> GetAllCustomers()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var customers = new List<CustomerViewModel>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM dbo.Customer WHERE isDeleted = 0 ORDER BY Name";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    customers.Add(new CustomerViewModel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ID")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        DiscountRate = reader.GetDecimal(reader.GetOrdinal("DiscountRate")),
                        RemainingBalances = reader.GetDecimal(reader.GetOrdinal("RemainingBalance")),
                        AddressStreet = reader.GetString(reader.GetOrdinal("Address_Street")),
                        AddressCity = reader.GetString(reader.GetOrdinal("Address_City")),
                        AddressState = reader.GetString(reader.GetOrdinal("Address_State")),
                        AddressPostalCode = reader.GetString(reader.GetOrdinal("Address_PostalCode"))
                    });
                }
            }
        }
        return customers;
    }

    public void AddCustomer(CustomerViewModel model)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.CreateCustomer";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@Name", model.Name));
        command.Parameters.Add(new SqlParameter("@Phone", model.Phone));
        command.Parameters.Add(new SqlParameter("@Email", model.Email ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@DiscountRate", model.DiscountRate));
        command.Parameters.Add(new SqlParameter("@Street", model.AddressStreet ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@City", model.AddressCity ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@State", model.AddressState ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@PostalCode", model.AddressPostalCode ?? (object)DBNull.Value));

        var outputParam = new SqlParameter("@NewCustomerID", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(outputParam);

        command.ExecuteNonQuery();
    }

    public CustomerViewModel GetCustomerById(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM dbo.Customer WHERE ID = @CustomerID AND isDeleted = 0";
        command.Parameters.Add(new SqlParameter("@CustomerID", id));

        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                return new CustomerViewModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    DiscountRate = reader.GetDecimal(reader.GetOrdinal("DiscountRate")),
                    AddressStreet = reader.GetString(reader.GetOrdinal("Address_Street")),
                    AddressCity = reader.GetString(reader.GetOrdinal("Address_City")),
                    AddressState = reader.GetString(reader.GetOrdinal("Address_State")),
                    AddressPostalCode = reader.GetString(reader.GetOrdinal("Address_PostalCode"))
                };
            }
        }
        return null;
    }

    public void UpdateCustomer(CustomerViewModel model)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.UpdateCustomer";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@CustomerID", model.Id));
        command.Parameters.Add(new SqlParameter("@Name", model.Name ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Phone", model.Phone ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Email", model.Email ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@DiscountRate", model.DiscountRate));
        command.Parameters.Add(new SqlParameter("@Street", model.AddressStreet ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@City", model.AddressCity ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@State", model.AddressState ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@PostalCode", model.AddressPostalCode ?? (object)DBNull.Value));

        command.ExecuteNonQuery();
    }

    public void DeleteCustomer(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.DeleteCustomer";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@CustomerID", id));

        command.ExecuteNonQuery();
    }
}
