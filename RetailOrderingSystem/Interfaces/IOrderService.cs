using RetailOrderingSystem.DTOs.Cart;
using RetailOrderingSystem.DTOs.Order;

namespace RetailOrderingSystem.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> PlaceOrderAsync(int customerId, PlaceOrderRequest request);
        Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int customerId);
        Task<OrderDto> GetOrderByIdAsync(int orderId, int requestingUserId, string role);
        Task<OrderDto> UpdateStatusAsync(int orderId, string status);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int page, int pageSize);
        Task<CartDto> ReorderAsync(int orderId, int customerId);
        Task AssignDeliveryPartnerAsync(int orderId, int deliveryPartnerId);
    }
}
