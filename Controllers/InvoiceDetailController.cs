using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagementWebApp.Models;

namespace SalesManagementWebApp.Controllers
{
    public class InvoiceDetailController : Controller
    {
        private readonly InvoiceDetailRepository _invoiceDetailRepository;
        private readonly ProductRepository _productRepository;

        public InvoiceDetailController(InvoiceDetailRepository invoiceDetailRepository, ProductRepository productRepository)
        {
            _invoiceDetailRepository = invoiceDetailRepository;
            _productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult Details(int invoiceId)
        {
            var details = _invoiceDetailRepository.GetInvoiceDetailsByInvoiceId(invoiceId);
            if (details == null || !details.Any())
            {
                return NotFound();
            }
            ViewData["InvoiceID"] = invoiceId;
            return View(details);
        }

        [HttpGet]
        public IActionResult Add(int invoiceId)
        {
            var products = _productRepository.GetAllProductList();

            var model = new InvoiceDetailViewModel
            {
                InvoiceID = invoiceId, 
                Products = products.Select(product => new SelectListItem
                {
                    Value = product.ID.ToString(),
                    Text = product.Name
                }).ToList()
            };

            return View(model);
        }



        [HttpPost]
        public IActionResult Add(InvoiceDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _invoiceDetailRepository.AddInvoiceDetail(model);
                    TempData["Message"] = "Fatura detayı başarıyla eklendi!";
                    return RedirectToAction("Details", new { invoiceId = model.InvoiceID });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Detay eklenirken bir hata oluştu: {ex.Message}");
                }
            }

            return View(model);
        }

        // Fatura detayı düzenleme (GET)
        [HttpGet]
        public IActionResult Edit(int detailId)
        {
            var detail = _invoiceDetailRepository.GetInvoiceDetailById(detailId);
            if (detail == null)
            {
                return NotFound();
            }

            return View(detail);
        }

        // Fatura detayı düzenleme (POST)
        [HttpPost]
        public IActionResult Edit(InvoiceDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _invoiceDetailRepository.UpdateInvoiceDetail(model);
                    TempData["Message"] = "Fatura detayı başarıyla güncellendi!";

                    return RedirectToAction("Detail", new { detailId = model.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Detay güncellenirken bir hata oluştu: {ex.Message}");
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int detailId)
        {
            try
            {
                var detail = _invoiceDetailRepository.GetInvoiceDetailById(detailId);
                if (detail == null)
                {
                    return NotFound();
                }

                _invoiceDetailRepository.DeleteInvoiceDetail(detailId);
                TempData["Message"] = "Fatura detayı başarıyla silindi!";

                return RedirectToAction("Detail", new { detailId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Detay silinirken bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("Detail", new { detailId });
        }
    }
}
