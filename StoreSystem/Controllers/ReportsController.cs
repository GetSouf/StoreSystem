using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Models;
using testproject.Models;
using OfficeOpenXml;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
namespace StoreSystem.Controllers

{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Страница с кнопками для выбора отчётов
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EmployeeReport(string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["SalesSortParam"] = sortOrder == "sales" ? "sales_desc" : "sales";
            ViewData["RevenueSortParam"] = sortOrder == "revenue" ? "revenue_desc" : "revenue";
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";

            // Фильтрация по датам
            var ordersQuery = _context.Orders
                .Include(o => o.Employee)
                .AsQueryable();

            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDate.Value);
            }

            var employeeReport = await ordersQuery
                .GroupBy(o => new { o.EmployeeId, o.Employee.FirstName })
                .Select(g => new EmployeeReportViewModel
                {
                    EmployeeName = g.Key.FirstName,
                    SalesCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    OrderDates = g.Select(o => o.OrderDate).ToList() // Даты заказов для сортировки
                })
                .ToListAsync();

            // Сортировка
            employeeReport = sortOrder switch
            {
                "name_desc" => employeeReport.OrderByDescending(e => e.EmployeeName).ToList(),
                "sales" => employeeReport.OrderBy(e => e.SalesCount).ToList(),
                "sales_desc" => employeeReport.OrderByDescending(e => e.SalesCount).ToList(),
                "revenue" => employeeReport.OrderBy(e => e.TotalRevenue).ToList(),
                "revenue_desc" => employeeReport.OrderByDescending(e => e.TotalRevenue).ToList(),
                "date" => employeeReport.OrderBy(e => e.OrderDates.FirstOrDefault()).ToList(),
                "date_desc" => employeeReport.OrderByDescending(e => e.OrderDates.FirstOrDefault()).ToList(),
                _ => employeeReport.OrderBy(e => e.EmployeeName).ToList(),
            };

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(employeeReport);
        }

        public IActionResult ExportToExcel()
        {
            var employeeReport = _context.Orders
                .Include(o => o.Employee)
                .GroupBy(o => new { o.EmployeeId, o.Employee.FirstName })
                .Select(g => new EmployeeReportViewModel
                {
                    EmployeeName = g.Key.FirstName,
                    SalesCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                })
                .ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Employee Report");

            // Заголовки
            worksheet.Cells[1, 1].Value = "Имя сотрудника";
            worksheet.Cells[1, 2].Value = "Кол-во продаж";
            worksheet.Cells[1, 3].Value = "Общий оборот";

            // Данные
            for (int i = 0; i < employeeReport.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = employeeReport[i].EmployeeName;
                worksheet.Cells[i + 2, 2].Value = employeeReport[i].SalesCount;
                worksheet.Cells[i + 2, 3].Value = employeeReport[i].TotalRevenue;
            }

            // Генерация файла
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "EmployeeReport.xlsx";

            return File(stream, contentType, fileName);
        }
        public IActionResult ExportToPdf()
        {
            var employeeReport = _context.Orders
                .Include(o => o.Employee)
                .GroupBy(o => new { o.EmployeeId, o.Employee.FirstName })
                .Select(g => new EmployeeReportViewModel
                {
                    EmployeeName = g.Key.FirstName,
                    SalesCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                })
                .ToList();

            using var stream = new MemoryStream();
            var document = new Document(PageSize.A4, 10, 10, 10, 10);
            var writer = PdfWriter.GetInstance(document, stream);

            document.Open();

            // Заголовок
            var fontTitle = FontFactory.GetFont("Arial", 16, Font.BOLD);
            document.Add(new Paragraph("Отчёт по сотрудникам", fontTitle));
            document.Add(new Paragraph(" "));

            // Таблица
            var table = new PdfPTable(3);
            table.AddCell("Имя сотрудника");
            table.AddCell("Кол-во продаж");
            table.AddCell("Общий оборот");

            foreach (var item in employeeReport)
            {
                table.AddCell(item.EmployeeName);
                table.AddCell(item.SalesCount.ToString());
                table.AddCell(item.TotalRevenue.ToString("C"));
            }

            document.Add(table);
            document.Close();

            stream.Position = 0;
            var contentType = "application/pdf";
            var fileName = "EmployeeReport.pdf";

            return File(stream, contentType, fileName);
        }
        public async Task<IActionResult> SalesReport(string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            ViewData["OrderIdSortParam"] = string.IsNullOrEmpty(sortOrder) ? "orderid_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["CustomerSortParam"] = sortOrder == "customer" ? "customer_desc" : "customer";
            ViewData["EmployeeSortParam"] = sortOrder == "employee" ? "employee_desc" : "employee";
            ViewData["TotalAmountSortParam"] = sortOrder == "total" ? "total_desc" : "total";
            ViewData["ProductCountSortParam"] = sortOrder == "product_count" ? "product_count_desc" : "product_count";

            var salesQuery = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .AsQueryable();

            if (startDate.HasValue)
                salesQuery = salesQuery.Where(o => o.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                salesQuery = salesQuery.Where(o => o.OrderDate <= endDate.Value);

            var salesReport = await salesQuery
                .Select(o => new SalesReportViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    EmployeeName = o.Employee.FirstName + " " + o.Employee.LastName,
                    ProductCount = o.OrderDetails.Sum(od => od.Quantity),
                    TotalAmount = o.TotalAmount,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailViewModel
                    {
                        ProductName = od.Product.Name,
                        Quantity = od.Quantity,
                        Price = od.Price
                    }).ToList()
                })
                .ToListAsync();

            // Сортировка
            salesReport = sortOrder switch
            {
                "orderid_desc" => salesReport.OrderByDescending(s => s.OrderId).ToList(),
                "date" => salesReport.OrderBy(s => s.OrderDate).ToList(),
                "date_desc" => salesReport.OrderByDescending(s => s.OrderDate).ToList(),
                "customer" => salesReport.OrderBy(s => s.CustomerName).ToList(),
                "customer_desc" => salesReport.OrderByDescending(s => s.CustomerName).ToList(),
                "employee" => salesReport.OrderBy(s => s.EmployeeName).ToList(),
                "employee_desc" => salesReport.OrderByDescending(s => s.EmployeeName).ToList(),
                "total" => salesReport.OrderBy(s => s.TotalAmount).ToList(),
                "total_desc" => salesReport.OrderByDescending(s => s.TotalAmount).ToList(),
                "product_count" => salesReport.OrderBy(s => s.ProductCount).ToList(),
                "product_count_desc" => salesReport.OrderByDescending(s => s.ProductCount).ToList(),
                _ => salesReport.OrderBy(s => s.OrderId).ToList(),
            };

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(salesReport);
        }
        public async Task<IActionResult> ProductSalesReport(string sortOrder)
        {
            // Параметры для сортировки
            ViewData["ProductSortParam"] = string.IsNullOrEmpty(sortOrder) ? "product_desc" : "";
            ViewData["CategorySortParam"] = sortOrder == "category" ? "category_desc" : "category";
            ViewData["SupplierSortParam"] = sortOrder == "supplier" ? "supplier_desc" : "supplier";
            ViewData["PriceSortParam"] = sortOrder == "price" ? "price_desc" : "price";
            ViewData["QuantitySortParam"] = sortOrder == "quantity" ? "quantity_desc" : "quantity";
            ViewData["SalesCountSortParam"] = sortOrder == "sales_count" ? "sales_count_desc" : "sales_count";
            ViewData["AverageRatingSortParam"] = sortOrder == "rating" ? "rating_desc" : "rating"; // Новый параметр

            // Запрос с использованием навигационных свойств
            var productSalesQuery = _context.OrderDetails
                .Include(od => od.Product)
                    .ThenInclude(p => p.Category)
                .Include(od => od.Product)
                    .ThenInclude(p => p.ProductSuppliers)
                        .ThenInclude(ps => ps.Supplier)
                .Include(od => od.Product)
                    .ThenInclude(p => p.Ratings)
                .GroupBy(od => new
                {
                    ProductName = od.Product.Name,
                    CategoryName = od.Product.Category.Name,
                    SupplierName = od.Product.ProductSuppliers
                        .Select(ps => ps.Supplier.Name).FirstOrDefault(),
                    od.Price,
                    AverageRating = od.Product.Ratings.Any()
                        ? od.Product.Ratings.Average(r => r.RatingValue)
                        : 0.0
                })
                .AsEnumerable()
                .Select(g => new ProductSalesReportViewModel
                {
                    ProductName = g.Key.ProductName,
                    CategoryName = g.Key.CategoryName,
                    SupplierName = g.Key.SupplierName ?? "Не указан",
                    Price = g.Key.Price,
                    QuantitySold = g.Sum(od => od.Quantity),
                    SalesCount = g.Count(),
                    AverageRating = Math.Round(g.Key.AverageRating, 2)
                });

            // Сортировка
            productSalesQuery = sortOrder switch
            {
                "product_desc" => productSalesQuery.OrderByDescending(p => p.ProductName),
                "category" => productSalesQuery.OrderBy(p => p.CategoryName),
                "category_desc" => productSalesQuery.OrderByDescending(p => p.CategoryName),
                "supplier" => productSalesQuery.OrderBy(p => p.SupplierName),
                "supplier_desc" => productSalesQuery.OrderByDescending(p => p.SupplierName),
                "price" => productSalesQuery.OrderBy(p => p.Price),
                "price_desc" => productSalesQuery.OrderByDescending(p => p.Price),
                "quantity" => productSalesQuery.OrderBy(p => p.QuantitySold),
                "quantity_desc" => productSalesQuery.OrderByDescending(p => p.QuantitySold),
                "sales_count" => productSalesQuery.OrderBy(p => p.SalesCount),
                "sales_count_desc" => productSalesQuery.OrderByDescending(p => p.SalesCount),
                "rating" => productSalesQuery.OrderBy(p => p.AverageRating), // Сортировка по средней оценке
                "rating_desc" => productSalesQuery.OrderByDescending(p => p.AverageRating), // Обратная сортировка
                _ => productSalesQuery.OrderBy(p => p.ProductName),
            };

            var productSales = productSalesQuery.ToList();
            return View(productSales);
        }




    }
}
