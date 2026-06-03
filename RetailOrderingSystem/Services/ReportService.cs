/*
 * Folder: Services
 * File: ReportService.cs
 * Purpose: Generates business reports — dashboard stats, sales reports, top products.
 * Who Calls It: ReportController
 */

using Microsoft.EntityFrameworkCore;
using RetailOrderingSystem.Constants;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.DTOs.Report;
using RetailOrderingSystem.Interfaces;

namespace RetailOrderingSystem.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders
                .Where(o => o.OrderStatus == AppConstants.StatusDelivered)
                .SumAsync(o => (decimal?)o.FinalAmount) ?? 0;
            var totalCustomers = await _context.Users
                .CountAsync(u => u.Role == AppConstants.RoleCustomer && !u.IsDeleted);
            var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
            var pendingOrders = await _context.Orders
                .CountAsync(o => o.OrderStatus == AppConstants.StatusPending);
            var deliveredOrders = await _context.Orders
                .CountAsync(o => o.OrderStatus == AppConstants.StatusDelivered);
            var lowStockProducts = await _context.Products
                .CountAsync(p => p.IsActive && p.StockQuantity <= p.MinimumStockLevel);

            var topProducts = await GetTopProductsAsync(5);

            return new DashboardDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalCustomers = totalCustomers,
                TotalProducts = totalProducts,
                PendingOrders = pendingOrders,
                DeliveredOrders = deliveredOrders,
                LowStockProducts = lowStockProducts,
                TopSellingProducts = topProducts.ToList()
            };
        }

        public async Task<SalesReportDto> GetDailySalesAsync(DateTime date)
        {
            var orders = await _context.Orders
                .Where(o => o.OrderDate.Date == date.Date)
                .ToListAsync();

            return new SalesReportDto
            {
                Date = date,
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.FinalAmount)
            };
        }

        public async Task<SalesReportDto> GetMonthlySalesAsync(int year, int month)
        {
            var orders = await _context.Orders
                .Where(o => o.OrderDate.Year == year && o.OrderDate.Month == month)
                .ToListAsync();

            return new SalesReportDto
            {
                Date = new DateTime(year, month, 1),
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.FinalAmount)
            };
        }

        public async Task<IEnumerable<SalesReportDto>> GetRevenueByRangeAsync(DateTime from, DateTime to)
        {
            var orders = await _context.Orders
                .Where(o => o.OrderDate.Date >= from.Date && o.OrderDate.Date <= to.Date)
                .ToListAsync();

            return orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new SalesReportDto
                {
                    Date = g.Key,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.FinalAmount)
                })
                .OrderBy(r => r.Date);
        }

        public async Task<IEnumerable<TopProductDto>> GetTopProductsAsync(int count = 10)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .Where(oi => oi.Order!.OrderStatus == AppConstants.StatusDelivered)
                .GroupBy(oi => new { oi.ProductId, oi.Product!.Name })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<TopCategoryDto>> GetTopCategoriesAsync()
        {
            return await _context.OrderItems
                .Include(oi => oi.Product).ThenInclude(p => p!.Category)
                .Include(oi => oi.Order)
                .Where(oi => oi.Order!.OrderStatus == AppConstants.StatusDelivered)
                .GroupBy(oi => new { oi.Product!.CategoryId, oi.Product.Category!.Name })
                .Select(g => new TopCategoryDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice),
                    TotalOrderItems = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(c => c.TotalRevenue)
                .ToListAsync();
        }
    }
}
