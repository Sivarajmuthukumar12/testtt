/*
 * Folder: Services
 * File: ProductService.cs
 * Purpose: Business logic for product management — CRUD, search, filter, toggle active.
 * Who Calls It: ProductController
 */

using Microsoft.EntityFrameworkCore;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.DTOs.Product;
using RetailOrderingSystem.Interfaces;
using RetailOrderingSystem.Models;

namespace RetailOrderingSystem.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(string? search, int? categoryId,
            decimal? minPrice, decimal? maxPrice)
        {
            // Build query dynamically based on filters
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await query.Select(p => MapToDto(p)).ToListAsync();
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new KeyNotFoundException($"Product with ID {id} not found.");

            return MapToDto(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductRequest request)
        {
            // Validate category exists
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
                throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                MinimumStockLevel = request.MinimumStockLevel,
                ImageUrl = request.ImageUrl,
                CategoryId = request.CategoryId,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(product.Id);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductRequest request)
        {
            var product = await _context.Products.FindAsync(id)
                ?? throw new KeyNotFoundException($"Product with ID {id} not found.");

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.MinimumStockLevel = request.MinimumStockLevel;
            product.ImageUrl = request.ImageUrl;
            product.CategoryId = request.CategoryId;
            product.IsActive = request.IsActive;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(product.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id)
                ?? throw new KeyNotFoundException($"Product with ID {id} not found.");

            // Soft delete — mark inactive instead of removing from DB
            product.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task<ProductDto> ToggleActiveAsync(int id)
        {
            var product = await _context.Products.FindAsync(id)
                ?? throw new KeyNotFoundException($"Product with ID {id} not found.");

            product.IsActive = !product.IsActive;
            await _context.SaveChangesAsync();
            return await GetByIdAsync(product.Id);
        }

        public async Task<IEnumerable<ProductDto>> GetLowStockAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.StockQuantity <= p.MinimumStockLevel)
                .Select(p => MapToDto(p))
                .ToListAsync();
        }

        private static ProductDto MapToDto(Product p) => new()
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
            CategoryName = p.Category?.Name ?? string.Empty
        };
    }
}
