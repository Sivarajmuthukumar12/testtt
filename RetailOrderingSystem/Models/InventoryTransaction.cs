/*
 * Folder: Models
 * File: InventoryTransaction.cs
 * Purpose: Audit log of every stock change for a product.
 * Who Uses It: AppDbContext, InventoryService, OrderService
 * Interview Tip: This is an audit trail pattern — never delete history, only add records.
 */

namespace RetailOrderingSystem.Models
{
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
        public string TransactionType { get; set; } = string.Empty;  // "OrderDeduction", "StockAdded", "Adjustment"
        public string? Notes { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
