using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recrut.API.DTOs;
using Recrut.Business.Services.Interfaces;
using Recrut.Data.Repositories.Interfaces;
using Recrut.Models;
using System.ComponentModel.DataAnnotations;

namespace Recrut.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        public UsersController(
            IPasswordHasher passwordHasher,
            IUserRepository userRepository)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
        }

        [HttpGet("byIds")]
        public async Task<IActionResult> GetUsersByIds([FromQuery] IEnumerable<int> ids)
        {
            var users = await _userRepository.GetByIdsAsync(ids);
            
            if (users == null || !users.Any())
                return NotFound(new OperationResult { Success = false, Message = "User(s) not found." });
            
            return Ok(users);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            if (!new EmailAddressAttribute().IsValid(email))
                return BadRequest(new OperationResult { Success = false, Message = "Invalid email format." });

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound(new OperationResult { Success = false, Message = "User not found." });

            return Ok(user);
        }

        [HttpPost("createUsers")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUsers([FromBody] IEnumerable<UserCreateDto> usersDto)
        {
            foreach (var userDto in usersDto)
            {
                if (!new EmailAddressAttribute().IsValid(userDto.Email))
                    return BadRequest(new OperationResult { Success = false, Message = "Invalid email format." });
            }

            // TODO : Automapper
            var users = usersDto.Select(dto => new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _passwordHasher.HashPassword(dto.PasswordHash)
            }).ToList();

            await _userRepository.CreateAsync(users);
            return Ok(new OperationResult { Success = true, Message = "User(s) created successfully." });
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUsers([FromBody] IEnumerable<User> users)
        {
            await _userRepository.DeleteAsync(users);
            return Ok(new OperationResult { Success = true, Message = "Deletion successful." });
        }

        [HttpDelete("byIds")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserByIds([FromBody] IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(new OperationResult { Success = false, Message = "No Id was sent." });

            int rowsAffected = await _userRepository.DeleteByIdsAsync(ids);
            if (rowsAffected == 0)
                return NotFound(new OperationResult { Success = false, Message = "User(s) not found." });

            return Ok(new OperationResult { Success = true, Message = "Deletion successful." });
        }

        [HttpDelete("{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserByEmail(string email)
        {
            if (!new EmailAddressAttribute().IsValid(email))
                return BadRequest(new OperationResult { Success = false, Message = "Invalid email format." });

            int rowsAffected = await _userRepository.DeleteUserByEmailAsync(email);
            if (rowsAffected == 0)
                return NotFound(new OperationResult { Success = false, Message = "User not found." });

            return Ok(new OperationResult { Success = true, Message = "User deletion successful." });
        }
    }
}