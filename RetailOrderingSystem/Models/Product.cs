/*
 * Folder: Models
 * File: Product.cs
 * Purpose: Represents a product (e.g., Margherita Pizza, Pepsi, Garlic Bread).
 * Who Uses It: AppDbContext, ProductService, CartService, OrderService
 * Interview Tip: Product has a foreign key CategoryId — Many Products belong to One Category.
 */

namespace RetailOrderingSystem.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumStockLevel { get; set; } = 5;  // Alert when stock falls below this
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Key — links this product to a category
        public int CategoryId { get; set; }
        public Category? Category { get; set; }  // Navigation property

        // Navigation Properties
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
