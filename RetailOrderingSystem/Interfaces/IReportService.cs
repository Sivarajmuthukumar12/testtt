using RetailOrderingSystem.DTOs.Report;

namespace RetailOrderingSystem.Interfaces
{
    public interface IReportService
    {
        Task<DashboardDto> GetDashboardAsync();
        Task<SalesReportDto> GetDailySalesAsync(DateTime date);
        Task<SalesReportDto> GetMonthlySalesAsync(int year, int month);
        Task<IEnumerable<SalesReportDto>> GetRevenueByRangeAsync(DateTime from, DateTime to);
        Task<IEnumerable<TopProductDto>> GetTopProductsAsync(int count = 10);
        Task<IEnumerable<TopCategoryDto>> GetTopCategoriesAsync();
    }
}
