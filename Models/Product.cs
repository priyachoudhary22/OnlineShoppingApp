// Defines the namespace for product-related models.
namespace OnlineShoppingApp.Models
{
    // Represents a product available in the online store.
    public class Product
    {
        // Stores the unique product ID.
        public int ProductId { get; set; }

        // Stores the product name.
        public string ProductName { get; set; } = string.Empty;

        // Stores the product category.
        public string Category { get; set; } = string.Empty;

        // Stores the original product price.
        public decimal Price { get; set; }

        // Stores the discount percentage.
        public decimal DiscountPercentage { get; set; }
    }
}