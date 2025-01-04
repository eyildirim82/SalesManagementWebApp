using Microsoft.AspNetCore.Mvc;
using SalesManagementWebApp.Models;
using System.Collections.Generic;

namespace SalesManagementWebApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentRepository _paymentRepository;

        public PaymentController(PaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public IActionResult Index()
        {
            var payments = _paymentRepository.GetAllPayments();
            return View(payments);
        }

        [HttpGet]
        public IActionResult Add(int? customerId = null)
        {
            var model = new PaymentViewModel
            {
                CustomerId = customerId ?? 0,
                PaymentDate = DateTime.Now,
                PaymentMethod = "Cash"
            };
            return View(model);

        }

        [HttpPost]
        public IActionResult Add(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.PaymentDate = DateTime.Now;
                model.PaymentMethod = "Cash";
                model.Amount = model.Amount != 0 ? model.Amount : 0m;
                _paymentRepository.AddPayment(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {

            var payment = _paymentRepository.GetPaymentById(id);
            if (payment == null) return NotFound();
            return View(payment);
        }

        [HttpPost]
        public IActionResult Edit(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                _paymentRepository.UpdatePayment(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _paymentRepository.DeletePayment(id);
            return RedirectToAction("Index");
        }
    }
}
