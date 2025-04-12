using Microsoft.AspNetCore.Mvc;
using Recrut.API.DTOs;
using Recrut.Business.Services.Interfaces;

namespace Recrut.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new OperationResult { Success = false, Message = "Données de connexion invalides." });
            }

            var (success, token) = await _authService.AuthenticateAsync(loginDto.Email, loginDto.Password);

            if (!success)
            {
                return Unauthorized(new OperationResult { Success = false, Message = "Email ou mot de passe invalide." });
            }

            return Ok(new AuthResultDto { Token = token });
        }
    }
}
