using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagementWebApp.Models;
using System;

namespace SalesManagementWebApp.Controllers
{
    public class StockMovementController : Controller
    {
        private readonly StockMovementRepository _stockMovementRepository;
        private readonly ProductRepository _productRepository;

        public StockMovementController(StockMovementRepository stockMovementRepository, ProductRepository productRepository)
        {
            _stockMovementRepository = stockMovementRepository;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var stockMovements = _stockMovementRepository.GetAllStockMovements();
            return View(stockMovements);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new StockMovementViewModel
            {
                Products = _productRepository.GetAllProductList()
                    .Select(c => new SelectListItem
                    {
                        Value = c.ID.ToString(),
                        Text = c.Name
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(StockMovementViewModel model)
        {
            model.Products = _productRepository.GetAllProductList()
                .Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name,
                    Selected = c.ID == model.ProductId
                }).ToList();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return View(model);
            }

            model.Date = DateTime.Now;

            _stockMovementRepository.AddStockMovement(model);
            return RedirectToAction("Index");
        }

        // GET: StockMovement/Edit/{id}
        public IActionResult Edit(int id)
        {
            var stockMovement = _stockMovementRepository.GetStockMovementById(id);
            if (stockMovement == null)
            {
                return NotFound();
            }

            var model = new StockMovementViewModel
            {
                Id = stockMovement.Id,
                ProductId = stockMovement.ProductId,
                MovementType = stockMovement.MovementType,
                Date = stockMovement.Date,
                Quantity = stockMovement.Quantity,
                Note = stockMovement.Note,
                Products = _productRepository.GetAllProductList()
                    .Select(c => new SelectListItem
                    {
                        Value = c.ID.ToString(),
                        Text = c.Name,
                        Selected = c.ID == stockMovement.ProductId
                    }).ToList()
            };
   

            return View(model);
        }

        // POST: StockMovement/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StockMovementViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Products = _productRepository.GetAllProductList()
                    .Select(c => new SelectListItem
                    {
                        Value = c.ID.ToString(),
                        Text = c.Name,
                        Selected = model.ProductId == c.ID
                    }).ToList();

                return View(model);
            }

            var stock = new StockMovementViewModel
            {
                Id = model.Id,
                MovementType = model.MovementType,
                Quantity = model.Quantity,
                ProductId = model.ProductId,
                Date = DateTime.Now,
                Note = model.Note
            };

            _stockMovementRepository.UpdateStockMovement(stock);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _stockMovementRepository.DeleteStockMovement(id);
                TempData["Message"] = "Stok hareketi başarıyla silindi!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Stok hareketi silinirken bir hata oluştu: {ex.Message}";
            }
            return RedirectToAction("Index");
        }
    }
}
