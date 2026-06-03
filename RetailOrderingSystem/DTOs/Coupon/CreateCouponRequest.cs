namespace RetailOrderingSystem.DTOs.Coupon
{
    public class CreateCouponRequest
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; } = 0;
        public decimal FixedDiscountAmount { get; set; } = 0;
        public decimal MinimumOrderAmount { get; set; } = 0;
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; } = 1;
    }
}
