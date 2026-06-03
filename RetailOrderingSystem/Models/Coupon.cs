/*
 * Folder: Models
 * File: Coupon.cs
 * Purpose: Represents a discount coupon that customers can apply at checkout.
 * Types: Percentage discount OR fixed amount discount.
 * Who Uses It: AppDbContext, CouponService, OrderService
 */

namespace RetailOrderingSystem.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;          // e.g., "SAVE20"
        public decimal DiscountPercentage { get; set; } = 0;      // e.g., 20 for 20% off
        public decimal FixedDiscountAmount { get; set; } = 0;     // e.g., 50 for ₹50 off
        public decimal MinimumOrderAmount { get; set; } = 0;      // Minimum cart value to apply
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; } = 1;                  // How many times it can be used
        public int UsedCount { get; set; } = 0;                   // How many times it has been used
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
