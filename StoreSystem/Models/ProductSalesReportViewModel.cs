namespace StoreSystem.Models
{
    public class ProductSalesReportViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantitySold { get; set; } // Общее количество проданных единиц товара
        public int SalesCount { get; set; }   // Количество продаж (заказов)
        public double AverageRating { get; set; } // Средняя оценка покупателей
    }
}
