using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models{
    public class Customer{
        [Key]
        public int CustomerId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}