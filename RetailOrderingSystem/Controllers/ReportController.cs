using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderingSystem.Interfaces;

namespace RetailOrderingSystem.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize(Roles = "Admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>Get admin dashboard statistics</summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _reportService.GetDashboardAsync();
            return Ok(result);
        }

        /// <summary>Get daily sales report</summary>
        [HttpGet("sales/daily")]
        public async Task<IActionResult> GetDailySales([FromQuery] DateTime date)
        {
            var result = await _reportService.GetDailySalesAsync(date);
            return Ok(result);
        }

        /// <summary>Get monthly sales report</summary>
        [HttpGet("sales/monthly")]
        public async Task<IActionResult> GetMonthlySales([FromQuery] int year, [FromQuery] int month)
        {
            var result = await _reportService.GetMonthlySalesAsync(year, month);
            return Ok(result);
        }

        /// <summary>Get revenue report by date range</summary>
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _reportService.GetRevenueByRangeAsync(from, to);
            return Ok(result);
        }

        /// <summary>Get top selling products</summary>
        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts([FromQuery] int count = 10)
        {
            var result = await _reportService.GetTopProductsAsync(count);
            return Ok(result);
        }

        /// <summary>Get top categories by revenue</summary>
        [HttpGet("top-categories")]
        public async Task<IActionResult> GetTopCategories()
        {
            var result = await _reportService.GetTopCategoriesAsync();
            return Ok(result);
        }
    }
}
