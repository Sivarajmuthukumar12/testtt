/*
 * Folder: Constants
 * File: AppConstants.cs
 * Purpose: Central place for all magic numbers and strings used across the app.
 *          Avoids hardcoding values in multiple places.
 * Interview Tip: Using constants instead of magic numbers is a SOLID principle (Open/Closed).
 */

namespace RetailOrderingSystem.Constants
{
    public static class AppConstants
    {
        // Roles
        public const string RoleAdmin = "Admin";
        public const string RoleCustomer = "Customer";
        public const string RoleDeliveryPartner = "DeliveryPartner";

        // Order Statuses
        public const string StatusPending = "Pending";
        public const string StatusAccepted = "Accepted";
        public const string StatusOutForDelivery = "OutForDelivery";
        public const string StatusDelivered = "Delivered";
        public const string StatusCancelled = "Cancelled";

        // Inventory Transaction Types
        public const string TransactionOrderDeduction = "OrderDeduction";
        public const string TransactionStockAdded = "StockAdded";
        public const string TransactionAdjustment = "Adjustment";

        // Loyalty Points Rules
        public const int LoyaltyPointsPerHundredRupees = 10;   // Earn 10 points per ₹100
        public const int LoyaltyPointsToRupeesDivisor = 100;   // 100 points = ₹10
        public const decimal LoyaltyRupeesPerHundredPoints = 10m; // ₹10 per 100 points

        // JWT
        public const int JwtExpiryMinutes = 60;
        public const int RefreshTokenExpiryDays = 7;
    }
}
