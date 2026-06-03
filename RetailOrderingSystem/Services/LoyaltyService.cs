/*
 * Folder: Services
 * File: LoyaltyService.cs
 * Purpose: Manages loyalty points — earn on purchase, redeem at checkout.
 * Rule: Earn 10 pts per ₹100. Redeem 100 pts = ₹10 discount.
 * Who Calls It: LoyaltyController, OrderService
 */

using Microsoft.EntityFrameworkCore;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.DTOs.Loyalty;
using RetailOrderingSystem.Interfaces;
using RetailOrderingSystem.Models;

namespace RetailOrderingSystem.Services
{
    public class LoyaltyService : ILoyaltyService
    {
        private readonly AppDbContext _context;

        public LoyaltyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LoyaltyPointDto> GetPointsAsync(int userId)
        {
            var loyalty = await _context.LoyaltyPoints
                .FirstOrDefaultAsync(lp => lp.UserId == userId);

            return new LoyaltyPointDto
            {
                UserId = userId,
                Points = loyalty?.Points ?? 0
            };
        }

        public async Task AddPointsAsync(int userId, int points)
        {
            var loyalty = await _context.LoyaltyPoints
                .FirstOrDefaultAsync(lp => lp.UserId == userId);

            if (loyalty == null)
            {
                loyalty = new LoyaltyPoint { UserId = userId, Points = points };
                _context.LoyaltyPoints.Add(loyalty);
            }
            else
            {
                loyalty.Points += points;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> RedeemPointsAsync(int userId, int points)
        {
            var loyalty = await _context.LoyaltyPoints
                .FirstOrDefaultAsync(lp => lp.UserId == userId);

            if (loyalty == null || loyalty.Points < points)
                return false;

            loyalty.Points -= points;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
