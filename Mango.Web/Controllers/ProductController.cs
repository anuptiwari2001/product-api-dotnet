using System.Text.Json;
using Mango.Web.Models;
using Mango.Web.Services.iServices;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class ProductController(IProductService productService) : Controller
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
        private readonly IProductService _productService = productService;

        public async Task<IActionResult> ProductIndex()
        {
            var response = await _productService.GetAllProductsAsync<ResponseDto>(string.Empty);
            var products = new List<ProductDto>();

            if (response is { IsSuccess: true, Result: not null })
            {
                products = JsonSerializer.Deserialize<List<ProductDto>>(response.Result.ToString()!, JsonOptions)
                    ?? throw new JsonException("The product API returned an empty product list response.");
            }
            else
            {
                var errorMessage = response?.ErrorMessages.FirstOrDefault(message => !string.IsNullOrWhiteSpace(message));
                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = response?.DisplayMessage;
                }

                ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(errorMessage)
                    ? "An error occurred while fetching products. Make sure the Product API is running."
                    : errorMessage);
            }

            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("ProductCreate", new ProductDto
            {
                Description = string.Empty,
                CategoryName = string.Empty
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _productService.GetProductByIdAsync<ResponseDto>(id, string.Empty);
            if (response is not { IsSuccess: true, Result: not null })
            {
                return NotFound();
            }

            var product = JsonSerializer.Deserialize<ProductDto>(response.Result.ToString()!, JsonOptions);
            return product is null ? NotFound() : View("ProductEdit", product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return View("ProductEdit", product);
            }

            var response = await _productService.UpdateProductAsync<ResponseDto>(product, string.Empty);
            if (response is { IsSuccess: true })
            {
                return RedirectToAction(nameof(ProductIndex));
            }

            AddResponseErrors(response, "An error occurred while updating the product.");
            return View("ProductEdit", product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "A valid product is required for deletion.";
                return RedirectToAction(nameof(ProductIndex));
            }

            var response = await _productService.DeleteProductAsync<ResponseDto>(id, string.Empty);
            if (response is { IsSuccess: true })
            {
                TempData["Success"] = "Product deleted successfully.";
            }
            else
            {
                TempData["Error"] = GetResponseError(response, "An error occurred while deleting the product.");
            }

            return RedirectToAction(nameof(ProductIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProductAsync<ResponseDto>(model, string.Empty);
                if (response is { IsSuccess: true })
                {
                    return RedirectToAction(nameof(ProductIndex));
                }

                AddResponseErrors(response, "An error occurred while creating the product.");
            }

            return View("ProductCreate", model);
        }

        private void AddResponseErrors(ResponseDto? response, string fallbackMessage)
        {
            var errors = response?.ErrorMessages?
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .ToList();

            if (errors is { Count: > 0 })
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return;
            }

            ModelState.AddModelError(string.Empty, GetResponseError(response, fallbackMessage));
        }

        private static string GetResponseError(ResponseDto? response, string fallbackMessage)
        {
            var errorMessage = response?.ErrorMessages?
                .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message));
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                return errorMessage;
            }

            return !string.IsNullOrWhiteSpace(response?.DisplayMessage)
                ? response.DisplayMessage
                : fallbackMessage;
        }
    }
}
