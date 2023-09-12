using blog.common.Exceptions;
using blog.common.Middleware;
using blog.common.Model;
using blog.common.Service;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// List all existing Users
        /// </summary>
        [HttpGet("")]
        [AuthorizeFilter, AllowAnonymousAttribute]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                return Ok(await _userService.ListAsync());
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }

        }

        /// <summary>
        /// Get a User by id
        /// </summary>
        [HttpGet("id/{userId}")]
        [AuthorizeFilter, AllowAnonymousAttribute]
        public async Task<IActionResult> GetByIdAsync([FromRoute] long userId)
        {
            try
            {
                return Ok(await _userService.GetByIdAsync(userId));
            }
            catch (NotFoundException e)
            {
                return NotFound(new ErrorMessage(e.Message));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }

        }

        /// <summary>
        /// Get a User by Username
        /// </summary>
        [HttpGet("username/{userName}")]
        [AuthorizeFilter, AllowAnonymousAttribute]
        public async Task<IActionResult> GetByUserNameAsync([FromRoute] string username)
        {
            try
            {
                return Ok(await _userService.GetByUsernameAsync(username));
            }
            catch (NotFoundException e)
            {
                return NotFound(new ErrorMessage(e.Message));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }

        }

        /// <summary>
        /// Save a new User
        /// </summary>
        [HttpPost("")]
        [AuthorizeFilter, AllowAnonymousAttribute]
        public async Task<IActionResult> PostAsync([FromBody] User user)
        {
            try
            {
                return Ok(await _userService.SaveAsync(user));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }

        }
    }
}