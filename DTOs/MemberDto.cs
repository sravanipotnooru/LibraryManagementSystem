using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    public class MemberDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }
    }

    public class MemberResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime MemberSince { get; set; }
    }
}
