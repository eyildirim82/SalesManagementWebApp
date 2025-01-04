namespace SalesManagementWebApp.Models
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
    }
}
