namespace StoreSystem.ViewModels
{
    public class EmployeeProfileViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Post { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public decimal Bonus { get; set; }


        public List<OrderViewModel> SalesHistory { get; set; } = new();
        public string ProfilePictureUrl { get; internal set; }
    }
}
