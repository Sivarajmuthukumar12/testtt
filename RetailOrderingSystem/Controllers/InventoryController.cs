using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderingSystem.DTOs.Inventory;
using RetailOrderingSystem.Interfaces;

namespace RetailOrderingSystem.Controllers
{
    [ApiController]
    [Route("api/inventory")]
    [Authorize(Roles = "Admin")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>Update stock quantity for a product</summary>
        [HttpPut("{productId}/stock")]
        public async Task<IActionResult> UpdateStock(int productId, [FromBody] UpdateStockRequest request)
        {
            await _inventoryService.UpdateStockAsync(productId, request);
            return Ok(new { Message = "Stock updated successfully." });
        }

        /// <summary>Get all low stock products</summary>
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock()
        {
            var result = await _inventoryService.GetLowStockProductsAsync();
            return Ok(result);
        }

        /// <summary>Get inventory transaction history for a product</summary>
        [HttpGet("{productId}/transactions")]
        public async Task<IActionResult> GetTransactions(int productId)
        {
            var result = await _inventoryService.GetTransactionHistoryAsync(productId);
            return Ok(result);
        }
    }
}
