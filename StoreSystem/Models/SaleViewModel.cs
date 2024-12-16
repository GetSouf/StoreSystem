using StoreSystem.Models;
using testproject.Models;

public class SaleViewModel
{
    public List<Customer> Customers { get; set; } = new(); 
    public List<Product> Products { get; set; } = new(); 
    public List<Category> Categories { get; set; } = new();
}