/*
 * Folder: Models
 * File: Cart.cs
 * Purpose: Represents a customer's shopping cart. One cart per customer.
 * Who Uses It: AppDbContext, CartService
 * Interview Tip: One-To-One relationship between User and Cart.
 */

namespace RetailOrderingSystem.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Key — each cart belongs to one user
        public int UserId { get; set; }
        public User? User { get; set; }

        // One Cart has Many CartItems
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
