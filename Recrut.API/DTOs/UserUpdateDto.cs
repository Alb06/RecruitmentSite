using System.ComponentModel.DataAnnotations;

namespace Recrut.API.DTOs
{
    public class UserUpdateDto
    {
        [Required]
        public required string Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? PasswordHash { get; set; }
    }
}
