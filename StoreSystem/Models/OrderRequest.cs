namespace StoreSystem.Models
{
    public class OrderRequest
    {
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
    }
}
