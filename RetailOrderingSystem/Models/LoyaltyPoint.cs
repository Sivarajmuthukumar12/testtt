/*
 * Folder: Models
 * File: LoyaltyPoint.cs
 * Purpose: Tracks loyalty points balance for each customer.
 * Rule: Earn 10 points per ₹100 spent. Redeem 100 points = ₹10 discount.
 * Who Uses It: AppDbContext, LoyaltyService, OrderService
 */

namespace RetailOrderingSystem.Models
{
    public class LoyaltyPoint
    {
        public int Id { get; set; }
        public int Points { get; set; } = 0;

        // Foreign Key — One-To-One with User
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
