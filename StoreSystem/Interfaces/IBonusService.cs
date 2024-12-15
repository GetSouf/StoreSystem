using StoreSystem.Models;

namespace StoreSystem.Interfaces
{
    public interface IBonusService
    {
        void AddBonus(int employeeId, decimal bonusAmount);
        decimal CalculateBonus(Product product);
    }
}
