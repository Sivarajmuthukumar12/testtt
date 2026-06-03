namespace RetailOrderingSystem.DTOs.Inventory
{
    public class UpdateStockRequest
    {
        public int NewQuantity { get; set; }
        public string TransactionType { get; set; } = "Adjustment";
        public string? Notes { get; set; }
    }

    public class InventoryTransactionDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
