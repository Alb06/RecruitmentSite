using Recrut.Models.Interfaces;

namespace Recrut.Models
{
    public class Role : IEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}