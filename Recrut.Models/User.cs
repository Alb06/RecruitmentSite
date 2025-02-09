using Recrut.Models.Interfaces;

namespace Recrut.Models
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
    }
}
