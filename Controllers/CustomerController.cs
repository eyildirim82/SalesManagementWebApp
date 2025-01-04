using Microsoft.AspNetCore.Mvc;
using SalesManagementWebApp.Models;

namespace SalesManagementWebApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerRepository _customerRepository;

        public CustomerController(CustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public IActionResult Index()
        {
            var customers = _customerRepository.GetAllCustomers();
            return View(customers);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new CustomerViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                _customerRepository.AddCustomer(model);
                TempData["SuccessMessage"] = "Müşteri başarıyla eklendi.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var customer = _customerRepository.GetCustomerById(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost]
        public IActionResult Edit(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                _customerRepository.UpdateCustomer(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _customerRepository.DeleteCustomer(id);
            return RedirectToAction("Index");
        }
    }
}
