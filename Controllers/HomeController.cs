// Imports MVC functionality.
using Microsoft.AspNetCore.Mvc;

// Imports memory caching functionality.
using Microsoft.Extensions.Caching.Memory;

// Imports the Product model.
using OnlineShoppingApp.Models;

// Imports the Product extension method.
using OnlineShoppingApp.Extensions;

// Imports JSON serialization functionality.
using System.Text.Json;

namespace OnlineShoppingApp.Controllers
{
    // Defines the controller for the shopping application.
    public class HomeController : Controller
    {
        // Stores the memory cache service.
        private readonly IMemoryCache _cache;

        // Stores the HTTP client.
        private readonly HttpClient _httpClient;

        // Creates the controller using dependency injection.
        public HomeController(
            IMemoryCache cache,
            IHttpClientFactory httpClientFactory)
        {
            // Saves the memory cache object.
            _cache = cache;

            // Creates an HTTP client.
            _httpClient = httpClientFactory.CreateClient();
        }

        // Displays the product list.
        public IActionResult Index()
        {
            // Attempts to get products from memory cache.
            if (!_cache.TryGetValue(
                "ProductList",
                out List<Product>? products))
            {
                // Creates sample products.
                products = new List<Product>
                {
                    // Creates the first product.
                    new Product
                    {
                        ProductId = 1,
                        ProductName = "Laptop",
                        Category = "Electronics",
                        Price = 60000,
                        DiscountPercentage = 10
                    },

                    // Creates the second product.
                    new Product
                    {
                        ProductId = 2,
                        ProductName = "Smartphone",
                        Category = "Electronics",
                        Price = 30000,
                        DiscountPercentage = 15
                    },

                    // Creates the third product.
                    new Product
                    {
                        ProductId = 3,
                        ProductName = "T-Shirt",
                        Category = "Clothing",
                        Price = 1500,
                        DiscountPercentage = 20
                    }
                };

                // Defines memory cache settings.
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    // Stores products in cache for ten minutes.
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(10)
                };

                // Stores the product list in memory cache.
                _cache.Set(
                    "ProductList",
                    products,
                    cacheOptions);
            }

            // Sends products to the view.
            return View(products);
        }

        // Displays details for one product.
        [ResponseCache(Duration = 60)]
        public IActionResult Details(int id)
        {
            // Retrieves products from memory cache.
            if (!_cache.TryGetValue(
                "ProductList",
                out List<Product>? products))
            {
                // Redirects to the product list.
                return RedirectToAction("Index");
            }

            // Finds the product using the query-string ID.
            Product? product =
                products.FirstOrDefault(
                    p => p.ProductId == id);

            // Returns 404 if the product doesn't exist.
            if (product == null)
            {
                return NotFound();
            }

            // Reads customer name from the cookie.
            string? customerName =
                Request.Cookies["CustomerName"];

            // Sends customer name to the view.
            ViewBag.CustomerName = customerName;

            // Calculates the discounted price.
            ViewBag.DiscountedPrice =
                product.GetDiscountedPrice();

            // Returns the details page.
            return View(product);
        }

        // Handles order submission.
        [HttpPost]
        public IActionResult PlaceOrder(
            int ProductId,
            string CustomerName)
        {
            // Stores the customer name in a cookie.
            Response.Cookies.Append(
                "CustomerName",
                CustomerName,
                new CookieOptions
                {
                    // Keeps the cookie for seven days.
                    Expires =
                        DateTimeOffset.Now.AddDays(7)
                });

            // Gets the cached product list.
            if (!_cache.TryGetValue(
                "ProductList",
                out List<Product>? products))
            {
                // Returns to the home page if cache is unavailable.
                return RedirectToAction("Index");
            }

            // Finds the selected product.
            Product? product =
                products.FirstOrDefault(
                    p => p.ProductId == ProductId);

            // Checks whether the product exists.
            if (product == null)
            {
                return NotFound();
            }

            // Stores the selected category in session.
            HttpContext.Session.SetString(
                "Category",
                product.Category);

            // Sends confirmation information to the view.
            ViewBag.Message =
                $"Order request submitted for {product.ProductName}.";

            // Reads the category from session.
            ViewBag.Category =
                HttpContext.Session.GetString("Category");

            // Displays confirmation.
            return View("Confirmation");
        }

        // Retrieves products asynchronously from Fake Store API.
        public async Task<IActionResult> ProductsApi()
        {
            // Sends an asynchronous request to the API.
            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    "https://fakestoreapi.com/products");

            // Throws an exception when the request fails.
            response.EnsureSuccessStatusCode();

            // Reads the response body asynchronously.
            string json =
                await response.Content.ReadAsStringAsync();

            // Converts JSON data to ProductApi objects.
            List<ProductApi>? products =
                JsonSerializer.Deserialize<List<ProductApi>>(
                    json,
                    new JsonSerializerOptions
                    {
                        // Ignores differences in JSON property capitalization.
                        PropertyNameCaseInsensitive = true
                    });

            // Sends API products to the view.
            return View(products);
        }
    }

    // Represents product data returned by Fake Store API.
    public class ProductApi
    {
        // Stores the API product ID.
        public int Id { get; set; }

        // Stores the API product title.
        public string Title { get; set; } = string.Empty;

        // Stores the API product price.
        public decimal Price { get; set; }

        // Stores the API product category.
        public string Category { get; set; } = string.Empty;
    }
}