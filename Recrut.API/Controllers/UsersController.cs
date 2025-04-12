using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        public UsersController(
            IMapper mapper,
            IPasswordHasher passwordHasher,
            IUserRepository userRepository
            )
        {
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
        }

        [HttpGet("byIds")]
        public async Task<IActionResult> GetUsersByIds([FromQuery] IEnumerable<int> ids)
        {
            var users = await _userRepository.GetByIdsAsync(ids);
            
            if (users == null || !users.Any())
                return NotFound(new OperationResult { Success = false, Message = "User(s) not found." });

            var usersDto = _mapper.Map<IEnumerable<UserResponseDto>>(users);

            return Ok(usersDto);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            if (!new EmailAddressAttribute().IsValid(email))
                return BadRequest(new OperationResult { Success = false, Message = "Invalid email format." });

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound(new OperationResult { Success = false, Message = "User not found." });

            var userDto = _mapper.Map<UserResponseDto>(user);
            return Ok(userDto);
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

            var users = _mapper.Map<IEnumerable<User>>(usersDto).ToList();
            foreach (var user in users)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash);
            }

            await _userRepository.CreateAsync(users);
            return Ok(new OperationResult { Success = true, Message = "User(s) created successfully." });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userDto)
        {
            if (userDto.Email != null && !new EmailAddressAttribute().IsValid(userDto.Email))
                return BadRequest(new OperationResult { Success = false, Message = "Invalid email format." });

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                return NotFound(new OperationResult { Success = false, Message = "User not found." });

            _mapper.Map(userDto, existingUser);

            if (!string.IsNullOrEmpty(userDto.PasswordHash))
            {
                existingUser.PasswordHash = _passwordHasher.HashPassword(userDto.PasswordHash);
            }

            await _userRepository.UpdateAsync(existingUser);
            return Ok(new OperationResult { Success = true, Message = "User updated successfully." });
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