using FashionAPI.Models.Request;
using FashionAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using FashionAPI.Enums;
using FashionAPI.Databases.FashionDB;
using FashionAPI.Models.BaseRequest;
using FashionAPI.Models.DataInfo;
using FashionAPI.Extensions;
using FashionAPI.Configuaration;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System;
using Microsoft.AspNetCore.Server.IISIntegration;

namespace FashionAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Dashboard Controller")]
    public class DashboardController : BaseController
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly DBContext _context;
        public DashboardController(DBContext context, ILogger<DashboardController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("dashboard_overview")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessage<DashboardOverviewDTO>), description: "DashboardOverview Response")]
        public async Task<IActionResult> DasboardOverview()
        {
            var response = new BaseResponseMessage<DashboardOverviewDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                response.Data = new DashboardOverviewDTO()
                {
                    TotalOrder = _context.Order.Where(x => x.Status == 1).Count(),
                    SuccessOrder = _context.Order.Where(x => x.State == 2 && x.Status == 1).Count(),
                    CancelOrder = _context.Order.Where(x => x.State == 3 && x.Status == 1).Count(),
                    TotalRevenue = _context.Order.Where(x => x.State == 2 && x.Status == 1).Sum(x => x.TotalPrice),
                    TotalRevenueByDay = _context.Order.Where(x => x.TimeCreated == DateTime.Now && x.State == 2 && x.Status == 1).Sum(b => b.TotalPrice),
                    TotalRevenueByMonth = _context.Order.Where(b => b.TimeCreated >= startOfMonth && b.TimeCreated <= endOfMonth && b.State == 2 && b.Status == 1).Sum(b => b.TotalPrice),
                    TotalRevenueByYear = _context.Order.Where(b => b.TimeCreated.Year == DateTime.Now.Year && b.State == 2 && b.Status == 1).Sum(b => b.TotalPrice)
                };
                return Ok(response);
            }
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }

        [HttpPost("dashboard_chart")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<DashboardChartDTO>), description: "DashboardChart Response")]
        public async Task<IActionResult> DashboardChart(DashboardChartRequest request)
        {
            var response = new BaseResponseMessageItem<DashboardChartDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                DateTime startDate, endDate;
                var today = DateTime.Today;

                switch (request.Filter)
                {
                    case 1: 
                        startDate = today.AddDays(-6);
                        endDate = today;
                        break;
                    case 2: 
                        startDate = new DateTime(today.Year, today.Month, 1);
                        endDate = startDate.AddMonths(1).AddDays(-1);
                        break;
                    case 3:
                        startDate = new DateTime(today.Year, 1, 1); 
                        endDate = new DateTime(today.Year, 12, 31); 
                        break;
                    default:
                        return BadRequest(response);
                }

                var query = _context.Order.Where(o => o.TimeCreated >= startDate && o.TimeCreated <= endDate && o.Status == 1)
                                            .Select(o => new { o.State, o.TimeCreated })
                                            .AsEnumerable(); // Đưa dữ liệu lên RAM để xử lý tiếp

                List<DashboardChartDTO> orderData = new List<DashboardChartDTO>();

                if (request.Filter == 1) // Lọc theo tuần (trả về ngày-tháng)
                {
                    var allDays = Enumerable.Range(0, 7)
                            .Select(i => startDate.AddDays(i).ToString("dd/MM"))
                            .ToList();
                    var groupedData = query
                    .GroupBy(o => o.TimeCreated.ToString("dd/MM"))
                    .ToDictionary(g => g.Key, g => new DashboardChartDTO
                    {
                        CountSuccess = g.Count(o => o.State == 2),
                        CountCancel = g.Count(o => o.State == 3),
                        Date = g.Key
                    });

                    orderData = allDays.Select(date => groupedData.ContainsKey(date) ? groupedData[date] : new DashboardChartDTO
                    {
                        CountSuccess = 0,
                        CountCancel = 0,
                        Date = date
                    }).OrderBy(g => g.Date).ToList();
                }
                else if (request.Filter == 2) // Lọc theo tháng (trả về tháng-năm)
                {
                    var allDays = Enumerable.Range(1, endDate.Day)
                            .Select(i => new DateTime(today.Year, today.Month, i).ToString("dd/MM"))
                            .ToList();

                    var groupedData = query
                        .GroupBy(o => o.TimeCreated.ToString("dd/MM"))
                        .ToDictionary(g => g.Key, g => new DashboardChartDTO
                        {
                            CountSuccess = g.Count(o => o.State == 2),
                            CountCancel = g.Count(o => o.State == 3),
                            Date = g.Key
                        });

                    orderData = allDays.Select(date => groupedData.ContainsKey(date) ? groupedData[date] : new DashboardChartDTO
                    {
                        CountSuccess = 0,
                        CountCancel = 0,
                        Date = date
                    }).OrderBy(g => g.Date).ToList();
                }
                else if (request.Filter == 3) 
                {
                    var allMonths = Enumerable.Range(1, 12)
                              .Select(i => new DateTime(today.Year, i, 1).ToString("MM/yyyy"))
                              .ToList();

                    var groupedData = query
                        .GroupBy(o => o.TimeCreated.ToString("MM/yyyy"))
                        .ToDictionary(g => g.Key, g => new DashboardChartDTO
                        {
                            CountSuccess = g.Count(o => o.State == 2),
                            CountCancel = g.Count(o => o.State == 3),
                            Date = g.Key
                        });

                    orderData = allMonths.Select(month => groupedData.ContainsKey(month) ? groupedData[month] : new DashboardChartDTO
                    {
                        CountSuccess = 0,
                        CountCancel = 0,
                        Date = month
                    }).OrderBy(g => g.Date).ToList();
                }
                response.Data = orderData;
                return Ok(response);
            }
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }

    }

}

