using RetailOrderingSystem.DTOs.Cart;

namespace RetailOrderingSystem.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task<CartDto> AddItemAsync(int userId, AddToCartRequest request);
        Task<CartDto> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemRequest request);
        Task<CartDto> RemoveItemAsync(int userId, int cartItemId);
        Task ClearCartAsync(int userId);
    }
}
