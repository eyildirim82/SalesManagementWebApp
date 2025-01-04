using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagementWebApp.Models;

namespace SalesManagementWebApp
{
    public class ProductViewModel
    {
        public int? ID { get; set; }

        public string Name { get; set; }

        public decimal ListPrice { get; set; }

        public decimal PurchasePrices { get; set; }

        public string? Description { get; set; }

        public int Quantity { get; set; }

        public decimal AvgSales { get; set; }

        public int? ProductCategoryID { get; set; }

        public string? CategoryName { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
    }

}