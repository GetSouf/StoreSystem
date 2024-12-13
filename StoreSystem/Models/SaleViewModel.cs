using StoreSystem.Models;
using testproject.Models;

public class SaleViewModel
{
    public List<Customer> Customers { get; set; } = new(); // Список существующих покупателей
    public List<Product> Products { get; set; } = new();  // На будущее: список товаров
    public List<Category> Categories { get; set; } = new();
}