using BudgetMaster.API.Contracts.User;
using BudgetMaster.IApplication.Models.DTO;
using BudgetMaster.IApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="request">The request containing user data.</param>
        /// <returns>The newly created user.</returns>
        /// <response code="200">Created. The new user is successfully created.</response>
        /// <response code="409">Conflict. The user already exists.</response>
        /// <response code="500">Internal Server Error. An unexpected error occurred.</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var createUserDto = new UserToCreateDto()
            {
                TelegramId = request.TelegramId,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };

            var createUserResult = await _userService.CreateUserAsync(createUserDto);
            
            if (createUserResult.IsFailure)
            {
                if(createUserResult.Error.Contains("failed", StringComparison.OrdinalIgnoreCase)) 
                    return StatusCode(StatusCodes.Status500InternalServerError, createUserResult.Error);

                return Conflict(createUserResult.Error);
            }
            //var response = new UserResponse()
            //{
            //    Id = createUserResult.Value.Id,
            //    TelegramId = createUserResult.Value.TelegramId,
            //    FirstName = createUserResult.Value.FirstName,
            //    LastName = createUserResult.Value.LastName,
            //};
            return Ok();
        }

        /// <summary>
        /// Retrieves a user by their Telegram ID.
        /// </summary>
        /// <param name="telegramId">The Telegram ID of the user.</param>
        /// <returns>The user with the specified Telegram ID.</returns>
        /// <response code="200">OK. The user is successfully retrieved.</response>
        /// <response code="404">Not Found. No user found for the specified Telegram ID.</response>
        [HttpGet("telegram/{telegramId}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserDto>> GetUserByTelegramId(long telegramId)
        {
            var getUserResult = await _userService.GetUserByTelegramIdAsync(telegramId);
            if (getUserResult.IsFailure)
            {
                return NotFound(getUserResult.Error);
            }
            return Ok(getUserResult.Value);
        }
    }
}
