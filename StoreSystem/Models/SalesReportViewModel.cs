namespace StoreSystem.Models
{
    public class SalesReportViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
