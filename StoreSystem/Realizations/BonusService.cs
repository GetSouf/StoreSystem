using StoreSystem.Interfaces;
using StoreSystem.Models;
using testproject.Models;

namespace StoreSystem.Realizations
{
    public class BonusService : IBonusService
    {
        private readonly ApplicationDbContext _context;

        public BonusService(ApplicationDbContext context)
        {
            _context = context;
        }
        public decimal CalculateBonus(Product product)
        {
            
            return product.Price * (decimal)(product.BonusPercentage / 100.0);
        }

        public void AddBonus(int employeeId, decimal bonusAmount)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null) throw new Exception("Сотрудник не найден.");

            employee.Bonus += bonusAmount;
            _context.SaveChanges();
        }
    }
}
