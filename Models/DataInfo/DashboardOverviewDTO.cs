namespace FashionAPI.Models.DataInfo
{
    public class DashboardOverviewDTO
    {
        public int TotalOrder { get; set; }
        public int SuccessOrder { get; set; }
        public int CancelOrder { get; set; }
        public double TotalRevenue { get; set; }
        public double TotalRevenueByDay { get; set; }
        public double TotalRevenueByMonth { get; set; }
        public double TotalRevenueByYear { get; set; }
    }
}
