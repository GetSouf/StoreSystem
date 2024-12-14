namespace StoreSystem.Models
{
    public class EmployeeReportViewModel
    {
        public string EmployeeName { get; set; }
        public int SalesCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<DateTime> OrderDates { get; set; } = new List<DateTime>();
    }
}
