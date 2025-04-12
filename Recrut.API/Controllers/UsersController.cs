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
    /// <summary>
    /// User management controller
    /// </summary>
    /// <remarks>
    /// This controller exposes CRUD operations for user management.
    /// Some methods are restricted to administrators.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the UsersController
        /// </summary>
        /// <param name="mapper">Mapping service between entities and DTOs</param>
        /// <param name="passwordHasher">Password hashing service</param>
        /// <param name="userRepository">Repository for user data access</param>
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

        /// <summary>
        /// Retrieves users by their specified IDs
        /// </summary>
        /// <param name="ids">List of user IDs to retrieve</param>
        /// <returns>List of found users</returns>
        /// <response code="200">Returns the list of found users</response>
        /// <response code="404">If no user is found for the specified IDs</response>
        [HttpGet("byIds")]
        [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsersByIds([FromQuery] IEnumerable<int> ids)
        {
            var validationResult = ValidateIds(ids);
            if (validationResult != null)
                return validationResult;

            var users = await _userRepository.GetByIdsAsync(ids);

            if (users == null || !users.Any())
                return NotFound(new OperationResult { Success = false, Message = "User(s) not found." });

            var usersDto = _mapper.Map<IEnumerable<UserResponseDto>>(users);

            return Ok(usersDto);
        }

        /// <summary>
        /// Retrieves a user by their email address
        /// </summary>
        /// <param name="email">The email address of the user to retrieve</param>
        /// <returns>The found user or 404 error if not found</returns>
        /// <response code="200">Returns the requested user</response>
        /// <response code="400">If the email format is invalid</response>
        /// <response code="404">If the user is not found</response>
        [HttpGet("{email}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var validationResult = ValidateEmail(email);
            if (validationResult != null)
                return validationResult;

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound(new OperationResult { Success = false, Message = "User not found." });

            var userDto = _mapper.Map<UserResponseDto>(user);
            return Ok(userDto);
        }

        /// <summary>
        /// Creates multiple users
        /// </summary>
        /// <param name="usersDto">User data to be created</param>
        /// <returns>Operation result</returns>
        /// <remarks>
        /// This operation is restricted to administrators.
        /// Passwords are automatically hashed before being stored.
        /// </remarks>
        /// <response code="200">If users were created successfully</response>
        /// <response code="400">If the data is invalid</response>
        /// <response code="409">If a conflict exists (e.g. email already used)</response>
        /// <response code="500">In case of an internal server error</response>
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
                var validationResult = ValidateEmail(userDto.Email);
                if (validationResult != null)
                    return validationResult;
            }

            var users = _mapper.Map<IEnumerable<User>>(usersDto).ToList();
            foreach (var user in users)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash);
            }

            await _userRepository.CreateAsync(users);
            return Ok(new OperationResult { Success = true, Message = "User(s) created successfully." });
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="id">ID of the user to update</param>
        /// <param name="userDto">New user data</param>
        /// <returns>Operation result</returns>
        /// <remarks>
        /// This operation is restricted to administrators.
        /// The password is automatically hashed if provided.
        /// </remarks>
        /// <response code="200">If the user was successfully updated</response>
        /// <response code="400">If the data is invalid</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="409">If a conflict exists (e.g. email already used)</response>
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userDto)
        {
            if (userDto.Email != null)
            {
                var emailValidationResult = ValidateEmail(userDto.Email);
                if (emailValidationResult != null)
                    return emailValidationResult;
            }

            var (userValidationResult, existingUser) = await GetAndValidateUserAsync(id);
            if (userValidationResult != null)
                return userValidationResult;

            _mapper.Map(userDto, existingUser);

            if (!string.IsNullOrEmpty(userDto.PasswordHash))
            {
                existingUser.PasswordHash = _passwordHasher.HashPassword(userDto.PasswordHash);
            }

            await _userRepository.UpdateAsync(existingUser);
            return Ok(new OperationResult { Success = true, Message = "User updated successfully." });
        }

        /// <summary>
        /// Deletes multiple users
        /// </summary>
        /// <param name="users">List of users to delete</param>
        /// <returns>Operation result</returns>
        /// <remarks>
        /// This operation is restricted to administrators.
        /// </remarks>
        /// <response code="200">If users were deleted successfully</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have the required permissions</response>
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteUsers([FromBody] IEnumerable<User> users)
        {
            await _userRepository.DeleteAsync(users);
            return Ok(new OperationResult { Success = true, Message = "Deletion successful." });
        }

        /// <summary>
        /// Deletes users by their IDs
        /// </summary>
        /// <param name="ids">List of user IDs to delete</param>
        /// <returns>Operation result</returns>
        /// <remarks>
        /// This operation is restricted to administrators.
        /// </remarks>
        /// <response code="200">If users were deleted successfully</response>
        /// <response code="400">If no ID is provided</response>
        /// <response code="404">If no user is found for the specified IDs</response>
        [HttpDelete("byIds")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUserByIds([FromBody] IEnumerable<int> ids)
        {
            var validationResult = ValidateIds(ids);
            if (validationResult != null)
                return validationResult;

            int rowsAffected = await _userRepository.DeleteByIdsAsync(ids);
            if (rowsAffected == 0)
                return NotFound(new OperationResult { Success = false, Message = "User(s) not found." });

            return Ok(new OperationResult { Success = true, Message = "Deletion successful." });
        }

        /// <summary>
        /// Deletes a user by their email address
        /// </summary>
        /// <param name="email">Email address of the user to delete</param>
        /// <returns>Operation result</returns>
        /// <remarks>
        /// This operation is restricted to administrators.
        /// </remarks>
        /// <response code="200">If the user was deleted successfully</response>
        /// <response code="400">If the email format is invalid</response>
        /// <response code="404">If the user is not found</response>
        [HttpDelete("{email}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUserByEmail(string email)
        {
            var validationResult = ValidateEmail(email);
            if (validationResult != null)
                return validationResult;

            int rowsAffected = await _userRepository.DeleteUserByEmailAsync(email);
            if (rowsAffected == 0)
                return NotFound(new OperationResult { Success = false, Message = "User not found." });

            return Ok(new OperationResult { Success = true, Message = "User deletion successful." });
        }

        #region Validation Methods

        /// <summary>
        /// Validates an email address format
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <returns>BadRequest result if invalid, null if valid</returns>
        private IActionResult ValidateEmail(string email)
        {
            if (!new EmailAddressAttribute().IsValid(email))
                return BadRequest(new OperationResult { Success = false, Message = "Invalid email format." });

            return null;
        }

        /// <summary>
        /// Validates that the collection of IDs is not null or empty
        /// </summary>
        /// <param name="ids">Collection of IDs to validate</param>
        /// <returns>BadRequest result if invalid, null if valid</returns>
        private IActionResult ValidateIds(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(new OperationResult { Success = false, Message = "No Id was sent." });

            return null;
        }

        /// <summary>
        /// Gets and validates that a user exists by ID
        /// </summary>
        /// <param name="id">User ID to check</param>
        /// <returns>Tuple with validation result and user (if found)</returns>
        private async Task<(IActionResult ValidationResult, User User)> GetAndValidateUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return (NotFound(new OperationResult { Success = false, Message = "User not found." }), null);

            return (null, user);
        }

        #endregion
    }
}