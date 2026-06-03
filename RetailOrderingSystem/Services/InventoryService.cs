/*
 * Folder: Services
 * File: InventoryService.cs
 * Purpose: Manages stock updates and maintains inventory transaction history.
 * Who Calls It: InventoryController, OrderService
 */

using Microsoft.EntityFrameworkCore;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.DTOs.Inventory;
using RetailOrderingSystem.DTOs.Product;
using RetailOrderingSystem.Interfaces;
using RetailOrderingSystem.Models;

namespace RetailOrderingSystem.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task UpdateStockAsync(int productId, UpdateStockRequest request)
        {
            var product = await _context.Products.FindAsync(productId)
                ?? throw new KeyNotFoundException($"Product with ID {productId} not found.");

            var oldQty = product.StockQuantity;

            // Record the transaction before updating
            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductId = productId,
                OldQuantity = oldQty,
                NewQuantity = request.NewQuantity,
                TransactionType = request.TransactionType,
                Notes = request.Notes
            });

            product.StockQuantity = request.NewQuantity;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.StockQuantity <= p.MinimumStockLevel)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    MinimumStockLevel = p.MinimumStockLevel,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category!.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTransactionDto>> GetTransactionHistoryAsync(int productId)
        {
            return await _context.InventoryTransactions
                .Include(t => t.Product)
                .Where(t => t.ProductId == productId)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new InventoryTransactionDto
                {
                    Id = t.Id,
                    ProductId = t.ProductId,
                    ProductName = t.Product!.Name,
                    OldQuantity = t.OldQuantity,
                    NewQuantity = t.NewQuantity,
                    TransactionType = t.TransactionType,
                    Notes = t.Notes,
                    TransactionDate = t.TransactionDate
                })
                .ToListAsync();
        }
    }
}
