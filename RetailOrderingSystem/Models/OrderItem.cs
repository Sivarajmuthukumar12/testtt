/*
 * Folder: Models
 * File: OrderItem.cs
 * Purpose: Represents a single product line in an order (snapshot of price at time of order).
 * Who Uses It: AppDbContext, OrderService
 */

namespace RetailOrderingSystem.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }  // Price at time of order (not current price)

        // Foreign Keys
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
