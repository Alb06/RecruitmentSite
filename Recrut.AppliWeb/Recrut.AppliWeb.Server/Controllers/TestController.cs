using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Recrut.AppliWeb.Server.Controllers
{
    [Route("test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "Le serveur BFF est accessible!",
                timestamp = DateTime.Now
            });
        }

        [HttpPost("login")]
        public IActionResult TestLogin([FromBody] object loginData)
        {
            try
            {
                _logger.LogInformation("Requête de test login reçue: {LoginData}", JsonSerializer.Serialize(loginData));

                return Ok(new
                {
                    success = true,
                    message = "Login de test réussi!",
                    token = "test_token_123456",
                    requestBody = loginData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement de la requête test login");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
