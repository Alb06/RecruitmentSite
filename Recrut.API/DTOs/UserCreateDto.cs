using System.ComponentModel.DataAnnotations;

namespace Recrut.API.DTOs
{
    public class UserCreateDto
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }
    }
}
