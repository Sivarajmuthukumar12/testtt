namespace RetailOrderingSystem.DTOs.Order
{
    public class PlaceOrderRequest
    {
        public string? CouponCode { get; set; }          // Optional coupon
        public int LoyaltyPointsToRedeem { get; set; } = 0;  // Optional loyalty redemption
        public string DeliveryAddress { get; set; } = string.Empty;
    }
}
