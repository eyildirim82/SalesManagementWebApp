using Microsoft.Data.SqlClient;
using SalesManagementWebApp;
using SalesManagementWebApp.Models;
using System.Data;

public class ProductRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProductRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IEnumerable<ProductViewModel> GetAllProducts()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var products = new List<ProductViewModel>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT p.ID, p.Name PName, p.ListPrice, p.Quantity," +
                "p.PurchasePrice,c.Name CName, p.AvgSales\r\nFROM dbo.Product p  join " +
                "dbo.ProductCategory c on p.ProductCategoryID=c.ID\r\nWHERE p.isDeleted = 0" +
                "\r\nOrder BY c.Name,p.Name";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    products.Add(new ProductViewModel
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                        Name = reader.GetString(reader.GetOrdinal("PName")),
                        CategoryName = reader.GetString(reader.GetOrdinal("CName")),
                        ListPrice = reader.GetDecimal(reader.GetOrdinal("ListPrice")),
                        PurchasePrices = reader.GetDecimal(reader.GetOrdinal("PurchasePrice")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        AvgSales = reader.GetDecimal(reader.GetOrdinal("AvgSales"))
                    });
                }
            }
        }

        return products;
    }

    public void CreateProduct(ProductViewModel model)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.CreateProduct";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@Name", model.Name));
        command.Parameters.Add(new SqlParameter("@ListPrice", model.ListPrice));
        command.Parameters.Add(new SqlParameter("@PurchasePrice", model.PurchasePrices));
        command.Parameters.Add(new SqlParameter("@Description", model.Description ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Quantity", model.Quantity));
        command.Parameters.Add(new SqlParameter("@AvgSales", model.AvgSales));
        command.Parameters.Add(new SqlParameter("@ProductCategoryID", model.ProductCategoryID));

        var outputParam = new SqlParameter("@NewProductID", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputParam);

        command.ExecuteNonQuery();

        var newProductId = (int)outputParam.Value;
    }

    public ProductViewModel GetProductById(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT ID, Name, ListPrice, PurchasePrice, Description, Quantity, AvgSales, ProductCategoryID FROM Product WHERE ID = @ID";
        command.Parameters.Add(new SqlParameter("@ID", id));

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new ProductViewModel
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                ListPrice = reader.GetDecimal(reader.GetOrdinal("ListPrice")),
                PurchasePrices = reader.GetDecimal(reader.GetOrdinal("PurchasePrice")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                AvgSales = reader.GetDecimal(reader.GetOrdinal("AvgSales")),
                ProductCategoryID = reader.IsDBNull(reader.GetOrdinal("ProductCategoryID")) ? null : reader.GetInt32(reader.GetOrdinal("ProductCategoryID"))
            };
        }

        return null;
    }

    public void UpdateProduct(ProductViewModel model)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.UpdateProduct"; 
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@ProductID", model.ID));
        command.Parameters.Add(new SqlParameter("@Name", model.Name ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@ListPrice", model.ListPrice));
        command.Parameters.Add(new SqlParameter("@PurchasePrice", model.PurchasePrices));
        command.Parameters.Add(new SqlParameter("@Description", model.Description ?? (object)DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Quantity", model.Quantity));
        command.Parameters.Add(new SqlParameter("@AvgSales", model.AvgSales));
        command.Parameters.Add(new SqlParameter("@ProductCategoryID", model.ProductCategoryID ?? (object)DBNull.Value));

        command.ExecuteNonQuery();
    }

    public void DeleteProduct(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "dbo.DeleteProduct"; 
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@ProductID", id));

        command.ExecuteNonQuery();
    }

    public IEnumerable<CategoryViewModel> GetAllCategories()
    {
        var categories = new List<CategoryViewModel>();

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT ID, Name FROM ProductCategory WHERE isDeleted = 0";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            categories.Add(new CategoryViewModel
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                Name = reader.GetString(reader.GetOrdinal("Name"))
            });
        }

        return categories;
    }

    public IEnumerable<ProductViewModel> GetAllProductList()
    {
        var products = new List<ProductViewModel>();

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT ID, Name FROM Product WHERE isDeleted = 0";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            products.Add(new ProductViewModel
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                Name = reader.GetString(reader.GetOrdinal("Name"))
            });
        }

        return products;
    }

}
