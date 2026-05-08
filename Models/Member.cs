using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Member
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        public DateTime MemberSince { get; set; } = DateTime.UtcNow;

        public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
    }
}
