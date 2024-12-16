namespace StoreSystem.Models
{
    public class EmployeeProfileViewModel
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string PostName { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public decimal Bonus { get; set; }
        public List<SalesReportViewModel> Orders { get; set; }
    }
}
