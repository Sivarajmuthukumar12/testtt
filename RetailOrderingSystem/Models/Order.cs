/*
 * Folder: Models
 * File: Order.cs
 * Purpose: Represents a confirmed customer order.
 * Status Flow: Pending → Accepted → OutForDelivery → Delivered
 * Who Uses It: AppDbContext, OrderService, DeliveryService
 */

namespace RetailOrderingSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }       // Sum before discount
        public decimal DiscountAmount { get; set; }    // Coupon or loyalty discount
        public decimal FinalAmount { get; set; }       // Amount customer pays
        public string OrderStatus { get; set; } = "Pending";
        public string? CouponCode { get; set; }
        public int LoyaltyPointsUsed { get; set; } = 0;
        public int LoyaltyPointsEarned { get; set; } = 0;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredDate { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public User? User { get; set; }

        public int? DeliveryPartnerId { get; set; }    // Assigned delivery partner
        public User? DeliveryPartner { get; set; }

        // One Order has Many OrderItems
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
