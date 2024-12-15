namespace StoreSystem.Models
{
    public class SalesReportViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }
}
