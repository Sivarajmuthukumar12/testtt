/*
 * Folder: Models
 * File: CartItem.cs
 * Purpose: Represents a single product line inside a cart.
 * Who Uses It: AppDbContext, CartService
 */

namespace RetailOrderingSystem.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        // Foreign Keys
        public int CartId { get; set; }
        public Cart? Cart { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
