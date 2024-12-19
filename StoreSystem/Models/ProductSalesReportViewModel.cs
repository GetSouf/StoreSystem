namespace StoreSystem.Models
{
    public class ProductSalesReportViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantitySold { get; set; } 
        public int SalesCount { get; set; }   
        public double AverageRating { get; set; } 
    }
}
