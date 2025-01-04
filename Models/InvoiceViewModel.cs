namespace SalesManagementWebApp.Models
{
    public class InvoiceViewModel
    {
        public int Id { get; set; } // Invoice ID
        public int CustomerId { get; set; } // Related Customer ID
        public string? CustomerName { get; set; } // Related Customer ID
        public string Type { get; set; } // Invoice Type
        public DateTime Date { get; set; } // Invoice Date
        public DateTime? DueDate { get; set; } // Due Date (Nullable)
        public string Status { get; set; } // Invoice Status
        public decimal ExchangeRate { get; set; } // Exchange Rate
        public decimal DiscountRate { get; set; } // Discount Rate
        public string Notes { get; set; } // Notes about the invoice
        public decimal TotalAmount { get; set; } // Total Amount of the Invoice
        public bool IsDeleted { get; set; } // Soft delete status
    }
}
