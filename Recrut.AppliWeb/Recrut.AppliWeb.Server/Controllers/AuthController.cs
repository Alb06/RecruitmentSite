using Microsoft.AspNetCore.Mvc;
using Recrut.AppliWeb.Server.Services.Interfaces;

namespace Recrut.AppliWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthProxyService _authProxyService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthProxyService authProxyService, ILogger<AuthController> logger)
        {
            _authProxyService = authProxyService;
            _logger = logger;
        }

        /// <summary>
        /// Authentifie un utilisateur et retourne un token JWT
        /// </summary>
        /// <param name="loginDto">Données de connexion</param>
        /// <returns>Token JWT</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] object loginDto)
        {
            try
            {
                _logger.LogInformation("Tentative de connexion reçue");

                // Proxifie la requête vers l'API backend
                var response = await _authProxyService.LoginAsync(loginDto);

                // Retourne la réponse avec le même code de statut
                var content = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la connexion");
                return StatusCode(500, new { success = false, message = "Une erreur s'est produite lors de la connexion" });
            }
        }
    }
}