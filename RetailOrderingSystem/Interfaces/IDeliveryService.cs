using RetailOrderingSystem.DTOs.Order;

namespace RetailOrderingSystem.Interfaces
{
    public interface IDeliveryService
    {
        Task<IEnumerable<OrderDto>> GetAssignedOrdersAsync(int deliveryPartnerId);
        Task<OrderDto> AcceptOrderAsync(int orderId, int deliveryPartnerId);
        Task<OrderDto> MarkDeliveredAsync(int orderId, int deliveryPartnerId);
    }
}
