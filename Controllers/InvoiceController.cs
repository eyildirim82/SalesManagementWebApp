using Microsoft.AspNetCore.Mvc;
using SalesManagementWebApp.Models;
using System;

namespace SalesManagementWebApp.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly InvoiceRepository _invoiceRepository;

        public InvoiceController(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public IActionResult Index()
        {
            var invoices = _invoiceRepository.GetAllInvoices();
            return View(invoices);
        }

        [HttpGet]
        public IActionResult Add(int? customerId = null)
        {
            var model = new InvoiceViewModel
            {
                CustomerId = customerId ?? 0, 
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                Status = "Unpaid",
                Type = "Sale" // Default type
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(InvoiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Date = DateTime.Now;
                    model.DueDate = DateTime.Now.AddDays(30);
                    model.Status = "Unpaid";

                    _invoiceRepository.AddInvoice(model);
                    TempData["Message"] = "Fatura başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Fatura eklenirken bir hata oluştu: {ex.Message}");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var invoice = _invoiceRepository.GetInvoiceById(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }

        [HttpPost]
        public IActionResult Edit(InvoiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _invoiceRepository.UpdateInvoice(model);
                    TempData["Message"] = "Fatura başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Fatura güncellenirken bir hata oluştu: {ex.Message}");
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _invoiceRepository.DeleteInvoice(id);
                TempData["Message"] = "Fatura başarıyla silindi!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fatura silinirken bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
