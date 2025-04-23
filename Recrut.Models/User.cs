using Recrut.Models.Interfaces;

namespace Recrut.Models
{
    public class User : IEntity, IAuditable
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}