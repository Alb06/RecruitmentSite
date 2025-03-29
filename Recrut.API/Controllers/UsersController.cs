using Microsoft.AspNetCore.Mvc;
using Recrut.API.DTOs;
using Recrut.Data.Repositories.Interfaces;
using Recrut.Models;
using System.ComponentModel.DataAnnotations;

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

        [HttpGet("byIds")]
        public async Task<IActionResult> GetUsersByIds([FromQuery] IEnumerable<int> ids)
        {
            try
            {
                var users = await _userRepository.GetByIdsAsync(ids);
                return Ok(users);
            }
            catch (Exception)
            {
                return StatusCode(500, new OperationResult { Success = false, Message = "An error occurred while retrieving users." });
            }
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            if (!new EmailAddressAttribute().IsValid(email))
                return BadRequest(new OperationResult { Success = false, Message = "Invalid email format." });

            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                    return NotFound(new OperationResult { Success = false, Message = "User not found." });
                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(500, new OperationResult { Success = false, Message = "An error occurred while retrieving the user." });
            }
        }

        [HttpPost]
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
                PasswordHash = dto.PasswordHash
            }).ToList();

            await _userRepository.CreateAsync(users);
            return Ok(new OperationResult { Success = true, Message = "User(s) created successfully." });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUsers([FromBody] IEnumerable<User> users)
        {
            try
            {
                await _userRepository.DeleteAsync(users);
                return Ok(new OperationResult { Success = true, Message = "Deletion successful." });
            }
            catch (Exception)
            {
                return StatusCode(500, new OperationResult { Success = false, Message = "An error occurred while deleting users." });
            }
        }

        [HttpDelete("byIds")]
        public async Task<IActionResult> DeleteUserByIds([FromBody] IEnumerable<int> Ids)
        {
            if (!Ids.Any() || Ids == null)
                return BadRequest(new OperationResult { Success = false, Message = "No Id was sent." });
            try
            {
                int rowsAffected = await _userRepository.DeleteByIdsAsync(Ids);
                if (rowsAffected == 0)
                    return NotFound(new OperationResult { Success = false, Message = "User(s) not found." });

                return Ok(new OperationResult { Success = true, Message = "Deletion successful." });
            }
            catch (Exception)
            {
                return StatusCode(500, new OperationResult { Success = false, Message = "An error occurred while deleting users." });
            }
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUserByEmail(string email)
        {
            if (!new EmailAddressAttribute().IsValid(email))
                return BadRequest(new OperationResult { Success = false, Message = "Invalid email format." });

            try
            {
                int rowsAffected = await _userRepository.DeleteUserByEmailAsync(email);
                if (rowsAffected == 0)
                    return NotFound(new OperationResult { Success = false, Message = "User not found." });

                return Ok(new OperationResult { Success = true, Message = "User deletion successful." });
            }
            catch (Exception)
            {
                return StatusCode(500, new OperationResult { Success = false, Message = "An error occurred while deleting the user." });
            }
        }
    }
}
