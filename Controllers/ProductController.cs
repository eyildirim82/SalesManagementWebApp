using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SalesManagementWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductRepository _productRepository;

        public ProductController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: Product/Index
        public IActionResult Index()
        {
            var products = _productRepository.GetAllProducts();
            return View(products);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            var model = new ProductViewModel
            {
                Categories = _productRepository.GetAllCategories()
                    .Select(c => new SelectListItem
                    {
                        Value = c.ID.ToString(),
                        Text = c.Name
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel model)
        {
            model.Categories = _productRepository.GetAllCategories()
                .Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name,
                    Selected = model.ProductCategoryID.HasValue && c.ID == model.ProductCategoryID.Value
                }).ToList();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return View(model);
            }

            var product = new ProductViewModel 
            {
                Name = model.Name,
                ListPrice = model.ListPrice,
                PurchasePrices = model.PurchasePrices,
                Quantity = model.Quantity,
                ProductCategoryID = model.ProductCategoryID.Value,
                Description = model.Description
            };

            _productRepository.CreateProduct(product);
            return RedirectToAction("Index");
        }

        // GET: Product/Edit/{id}
        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel
            {
                ID = product.ID,
                Name = product.Name,
                ListPrice = product.ListPrice,
                PurchasePrices = product.PurchasePrices,
                Quantity = product.Quantity,
                Description = product.Description,
                ProductCategoryID = product.ProductCategoryID,
                Categories = _productRepository.GetAllCategories()
                    .Select(c => new SelectListItem
                    {
                        Value = c.ID.ToString(),
                        Text = c.Name,
                        Selected = product.ProductCategoryID == c.ID
                    }).ToList()
            };

            return View(model);
        }

        // POST: Product/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = _productRepository.GetAllCategories()
                    .Select(c => new SelectListItem
                    {
                        Value = c.ID.ToString(),
                        Text = c.Name,
                        Selected = model.ProductCategoryID == c.ID
                    }).ToList();

                return View(model);
            }

            var product = new ProductViewModel
            {
                ID = model.ID,
                Name = model.Name,
                ListPrice = model.ListPrice,
                PurchasePrices = model.PurchasePrices,
                Quantity = model.Quantity,
                ProductCategoryID = model.ProductCategoryID.Value
            };

            _productRepository.UpdateProduct(product);
            return RedirectToAction("Index");
        }


        // POST: Product/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _productRepository.DeleteProduct(id);
                TempData["SuccessMessage"] = "Ürün başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
