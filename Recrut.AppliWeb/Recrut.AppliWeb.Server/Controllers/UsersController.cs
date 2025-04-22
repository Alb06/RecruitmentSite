using Microsoft.AspNetCore.Mvc;
using Recrut.AppliWeb.Server.Services.Interfaces;

namespace Recrut.AppliWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserProxyService _userProxyService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserProxyService userProxyService, ILogger<UsersController> logger)
        {
            _userProxyService = userProxyService;
            _logger = logger;
        }

        /// <summary>
        /// Extrait le token JWT de l'en-tête d'autorisation
        /// </summary>
        private string ExtractJwtToken()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            return authHeader.Substring("Bearer ".Length);
        }

        /// <summary>
        /// Récupère un utilisateur par son email
        /// </summary>
        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                _logger.LogInformation("Récupération de l'utilisateur avec l'email: {Email}", email);
                var token = ExtractJwtToken();

                var response = await _userProxyService.GetUserByEmailAsync(email, token);
                var content = await response.Content.ReadAsStringAsync();

                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur par email: {Email}", email);
                return StatusCode(500, new { success = false, message = "Une erreur s'est produite lors de la récupération de l'utilisateur" });
            }
        }

        /// <summary>
        /// Récupère des utilisateurs par leurs IDs
        /// </summary>
        [HttpGet("byIds")]
        public async Task<IActionResult> GetUsersByIds([FromQuery] IEnumerable<int> ids)
        {
            try
            {
                _logger.LogInformation("Récupération des utilisateurs avec les IDs: {IDs}", string.Join(", ", ids));
                var token = ExtractJwtToken();

                var response = await _userProxyService.GetUsersByIdsAsync(ids, token);
                var content = await response.Content.ReadAsStringAsync();

                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs par IDs");
                return StatusCode(500, new { success = false, message = "Une erreur s'est produite lors de la récupération des utilisateurs" });
            }
        }

        /// <summary>
        /// Crée de nouveaux utilisateurs
        /// </summary>
        [HttpPost("createUsers")]
        public async Task<IActionResult> CreateUsers([FromBody] object usersDto)
        {
            try
            {
                _logger.LogInformation("Création de nouveaux utilisateurs");
                var token = ExtractJwtToken();

                var response = await _userProxyService.CreateUsersAsync(usersDto, token);
                var content = await response.Content.ReadAsStringAsync();

                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création des utilisateurs");
                return StatusCode(500, new { success = false, message = "Une erreur s'est produite lors de la création des utilisateurs" });
            }
        }

        /// <summary>
        /// Met à jour un utilisateur
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] object userDto)
        {
            try
            {
                _logger.LogInformation("Mise à jour de l'utilisateur avec l'ID: {ID}", id);
                var token = ExtractJwtToken();

                var response = await _userProxyService.UpdateUserAsync(id, userDto, token);
                var content = await response.Content.ReadAsStringAsync();

                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'utilisateur avec l'ID: {ID}", id);
                return StatusCode(500, new { success = false, message = "Une erreur s'est produite lors de la mise à jour de l'utilisateur" });
            }
        }

        /// <summary>
        /// Supprime un utilisateur par son email
        /// </summary>
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUserByEmail(string email)
        {
            try
            {
                _logger.LogInformation("Suppression de l'utilisateur avec l'email: {Email}", email);
                var token = ExtractJwtToken();

                var response = await _userProxyService.DeleteUserByEmailAsync(email, token);
                var content = await response.Content.ReadAsStringAsync();

                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'utilisateur avec l'email: {Email}", email);
                return StatusCode(500, new { success = false, message = "Une erreur s'est produite lors de la suppression de l'utilisateur" });
            }
        }

        /// <summary>
        /// Supprime des utilisateurs par leurs IDs
        /// </summary>
        [HttpDelete("byIds")]
        public async Task<IActionResult> DeleteUsersByIds([FromBody] IEnumerable<int> ids)
        {
            try
            {
                _logger.LogInformation("Suppression des utilisateurs avec les IDs: {IDs}", string.Join(", ", ids));
                var token = ExtractJwtToken();

                var response = await _userProxyService.DeleteUsersByIdsAsync(ids, token);
                var content = await response.Content.ReadAsStringAsync();

                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression des utilisateurs par IDs");
                return StatusCode(500, new { success = false, message = "Une erreur s'est produite lors de la suppression des utilisateurs" });
            }
        }
    }
}