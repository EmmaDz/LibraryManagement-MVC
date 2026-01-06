using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models{
    public class LibraryBranch{
        [Key]
        public int BranchId { get; set; }
        public string? BranchName { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string Telephone { get; set; }
    }
}