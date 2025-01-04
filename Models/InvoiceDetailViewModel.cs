namespace SalesManagementWebApp.Models
{
    public class InvoiceDetailViewModel
    {
        public int Id { get; set; }
        public int InvoiceID { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
