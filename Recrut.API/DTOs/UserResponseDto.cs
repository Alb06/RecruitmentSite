namespace Recrut.API.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // Pas de PasswordHash pour raisons de sécurité

        // Optionnel : ajout d'informations sur les rôles
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
