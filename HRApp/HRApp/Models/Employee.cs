using System;

namespace HRApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullNameEn { get; set; }
        public string FullNameAr { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Gender { get; set; }          // Male / Female
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; }           // Active / Inactive
        public string Phone { get; set; }
        public string Email { get; set; }

        // Convenience: shows Arabic name if present, otherwise English
        public string DisplayName => string.IsNullOrWhiteSpace(FullNameAr) ? FullNameEn : FullNameAr;
    }
}
