// Imports the Product model.
using OnlineShoppingApp.Models;

// Defines the namespace for extension methods.
namespace OnlineShoppingApp.Extensions
{
    // Contains extension methods for Product objects.
    public static class ProductExtensions
    {
        // Calculates the product price after applying the discount.
        public static decimal GetDiscountedPrice(this Product product)
        {
            // Calculates the discount amount.
            decimal discount =
                product.Price * product.DiscountPercentage / 100;

            // Subtracts the discount from the original price.
            return product.Price - discount;
        }
    }
}