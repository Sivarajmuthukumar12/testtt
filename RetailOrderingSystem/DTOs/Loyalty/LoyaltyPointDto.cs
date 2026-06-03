namespace RetailOrderingSystem.DTOs.Loyalty
{
    public class LoyaltyPointDto
    {
        public int UserId { get; set; }
        public int Points { get; set; }
        public decimal EquivalentRupees => (Points / 100m) * 10m; // 100 pts = ₹10
    }

    public class RedeemPointsRequest
    {
        public int PointsToRedeem { get; set; }
    }
}
