using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Interfaces;
using StoreSystem.Models;
using StoreSystem.Realizations;
using testproject.Models;

namespace StoreSystem.Controllers
{
    [Authorize]
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBonusService _bonusService;

        public SaleController(ApplicationDbContext context, IBonusService bonusService)
        {
            _context = context;
            _bonusService = bonusService;
        }

        public IActionResult Sale()
        {
            var customers = _context.Customers.ToList();
            var categories = _context.Categories.ToList();

            var model = new SaleViewModel
            {
                Customers = customers,
                Categories = categories
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult GetProducts(string? search, int? categoryId)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.StockQuantity
            }).ToList();

            return Json(result);
        }
        [HttpGet]
        public IActionResult GetProductsByCategory(int categoryId)
        {
            var products = _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.StockQuantity
                })
                .ToList();

            return Json(products);


        }
        [HttpPost]
        public IActionResult CreateOrder(int customerId, int employeeId, [FromBody] List<OrderDetail> orderDetails)
        {
            Console.WriteLine($"CustomerId: {customerId}, EmployeeId: {employeeId}");
           

            if (orderDetails == null || !orderDetails.Any())
            {
                Console.WriteLine("OrderDetails is empty.");
                return BadRequest("Корзина пуста. Невозможно создать заказ.");
            }

            decimal totalAmount = orderDetails.Sum(od => od.Price * od.Quantity);
            Console.WriteLine($"Total Amount: {totalAmount}");

            var order = new Order
            {
                CustomerId = customerId,
                EmployeeId = employeeId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                OrderDetails = orderDetails
            };

            _context.Orders.Add(order);

            decimal bonusAmount = CalculateBonus(orderDetails);
            Console.WriteLine($"Calculated Bonus: {bonusAmount}");

            _bonusService.AddBonus(employeeId, bonusAmount);
            Console.WriteLine("Bonus added successfully.");

            _context.SaveChanges();
            Console.WriteLine("Order saved.");

            return Ok(new { orderId = order.Id, message = "Заказ успешно создан!" });
        }
        
        private decimal CalculateBonus(List<OrderDetail> orderDetails)
        {
            decimal totalBonus = 0;

            foreach (var detail in orderDetails)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == detail.ProductId);
                if (product != null)
                {
                    decimal productBonus = Math.Min(0.05m * (detail.Price * detail.Quantity), detail.Price * detail.Quantity);
                    totalBonus += productBonus;
                }
            }

            return totalBonus;
        }
        public IActionResult EmployeeBonus(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null) return NotFound();

            return View(employee);
        }
        public IActionResult SaleDetails(Product product)
        {
            var bonus = _bonusService.CalculateBonus(product);

            ViewBag.Bonus = bonus;
            return View(product);
        }



    }
}