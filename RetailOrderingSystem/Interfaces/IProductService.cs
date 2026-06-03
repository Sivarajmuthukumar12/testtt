using RetailOrderingSystem.DTOs.Product;

namespace RetailOrderingSystem.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice);
        Task<ProductDto> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductRequest request);
        Task<ProductDto> UpdateAsync(int id, UpdateProductRequest request);
        Task DeleteAsync(int id);
        Task<ProductDto> ToggleActiveAsync(int id);
        Task<IEnumerable<ProductDto>> GetLowStockAsync();
    }
}
