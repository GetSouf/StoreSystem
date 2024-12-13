using Microsoft.AspNetCore.Mvc;
using StoreSystem.Models;
using testproject.Models;

namespace StoreSystem.Controllers
{
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaleController(ApplicationDbContext context)
        {
            _context = context;
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
            if (orderDetails == null || !orderDetails.Any())
            {
                return BadRequest("Корзина пуста. Невозможно создать заказ.");
            }

            // Вычисляем общую сумму заказа
            decimal totalAmount = orderDetails.Sum(od => od.Price * od.Quantity);

            // Создаем заказ
            var order = new Order
            {
                CustomerId = customerId,
                EmployeeId = employeeId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                OrderDetails = orderDetails
            };

            order.OrderDate = DateTime.UtcNow;
            _context.Orders.Add(order);
            _context.SaveChanges();

            return Ok(new { orderId = order.Id, message = "Заказ успешно создан!" });
        }


    }
}