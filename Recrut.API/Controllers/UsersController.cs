using Microsoft.AspNetCore.Mvc;
using Recrut.API.DTOs;
using Recrut.Data.Repositories.Interfaces;
using Recrut.Models;

namespace Recrut.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            await _userRepository.CreateAsync(new List<User> { user });
            return CreatedAtAction(nameof(GetUserByEmail), new { email = user.Email }, user);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateUsers([FromBody] IEnumerable<User> users)
        //{
        //    await _userRepository.CreateAsync(users);
        //    return CreatedAtAction(nameof(GetAllUsers), null);
        //}

        [HttpDelete]
        public async Task<IActionResult> DeleteUsers([FromBody] IEnumerable<User> users)
        {
            await _userRepository.DeleteAsync(users);
            return NoContent();
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new OperationResult { Success = false, Message = "Utilisateur non trouvé." });
            }

            try
            {
                await _userRepository.DeleteAsync(new List<User> { user });
                return NoContent();  // ou: return Ok(new OperationResult { Success = true, Message = "Suppression réussie." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new OperationResult { Success = false, Message = $"Erreur lors de la suppression : {ex.Message}" });
            }
        }
    }
}
