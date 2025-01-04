using Microsoft.AspNetCore.Mvc.Rendering;

namespace SalesManagementWebApp.Models
{
    public class StockMovementViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string MovementType { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public IEnumerable<SelectListItem>? Products { get; set; }


    }
}
