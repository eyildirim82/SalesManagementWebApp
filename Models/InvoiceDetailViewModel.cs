using Microsoft.AspNetCore.Mvc.Rendering;

namespace SalesManagementWebApp.Models
{
    public class InvoiceDetailViewModel
    {
        public int? Id { get; set; }
        public int? InvoiceID { get; set; }
        public int? ProductID { get; set; }
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }

        public List<SelectListItem>? Products { get; set; }
    }

}
