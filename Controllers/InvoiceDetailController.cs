using Microsoft.AspNetCore.Mvc;
using SalesManagementWebApp.Models;

namespace SalesManagementWebApp.Controllers
{
    public class InvoiceDetailController : Controller
    {
        private readonly InvoiceDetailRepository _invoiceDetailRepository;

        public InvoiceDetailController(InvoiceDetailRepository invoiceDetailRepository)
        {
            _invoiceDetailRepository = invoiceDetailRepository;
        }

        public IActionResult Details(int invoiceId)
        {
            var invoiceDetails = _invoiceDetailRepository.GetInvoiceDetailsByInvoiceId(invoiceId)
                                ?? new List<InvoiceDetailViewModel>();

            return View(invoiceDetails);
        }

    }
}
