using RetailOrderingSystem.DTOs.Loyalty;

namespace RetailOrderingSystem.Interfaces
{
    public interface ILoyaltyService
    {
        Task<LoyaltyPointDto> GetPointsAsync(int userId);
        Task AddPointsAsync(int userId, int points);
        Task<bool> RedeemPointsAsync(int userId, int points);
    }
}
