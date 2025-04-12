using Microsoft.AspNetCore.Mvc;
using Recrut.API.DTOs;
using Recrut.Business.Services.Interfaces;

namespace Recrut.API.Controllers
{
    /// <summary>
    /// Authentication management controller
    /// </summary>
    /// <remarks>
    /// This controller handles operations related to user authentication,
    /// including login and JWT token generation.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the AuthController
        /// </summary>
        /// <param name="authService">Authentication service</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token
        /// </summary>
        /// <param name="loginDto">Login credentials (email and password)</param>
        /// <returns>JWT token upon successful authentication</returns>
        /// <remarks>
        /// The generated JWT token contains the user's claims,
        /// including their roles, and can be used to access protected resources.
        /// </remarks>
        /// <response code="200">Returns a valid JWT token</response>
        /// <response code="400">If the login data is invalid</response>
        /// <response code="401">If authentication fails (invalid email or password)</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new OperationResult { Success = false, Message = "Invalid login details." });
            }

            var (success, token) = await _authService.AuthenticateAsync(loginDto.Email, loginDto.Password);

            if (!success)
            {
                return Unauthorized(new OperationResult { Success = false, Message = "Invalid email or password." });
            }

            return Ok(new AuthResultDto { Token = token });
        }
    }
}
