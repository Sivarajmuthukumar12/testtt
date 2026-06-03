using RetailOrderingSystem.DTOs.Inventory;
using RetailOrderingSystem.DTOs.Product;

namespace RetailOrderingSystem.Interfaces
{
    public interface IInventoryService
    {
        Task UpdateStockAsync(int productId, UpdateStockRequest request);
        Task<IEnumerable<ProductDto>> GetLowStockProductsAsync();
        Task<IEnumerable<InventoryTransactionDto>> GetTransactionHistoryAsync(int productId);
    }
}
