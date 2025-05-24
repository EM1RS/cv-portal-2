using Microsoft.AspNetCore.Mvc;
using CvAPI2.Models;
using Microsoft.AspNetCore.Authorization;
using CvAPI2.DTO;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;

namespace CvAPI2.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        ///private readonly UserManager<User> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            try
            {
                var userDtos = await _userService.GetUsers();
                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feil ved henting av brukerliste.");
                return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        {
            try
            {
                var userDto = await _userService.GetUserById(id);
                return Ok(userDto);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Bruker med ID {Id} ble ikke funnet.", id);
                return NotFound(new ApiError { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feil ved henting av bruker med ID {Id}", id);
                return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto newUser)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { message = "Valideringsfeil", errors });
            }

            try
            {
                var createdUser = await _userService.CreateUser(newUser);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feil ved opprettelse av bruker");
                return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updatedUser)
        {
            try
            {
                await _userService.UpdateUser(id, updatedUser);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Bruker med ID {Id} ble ikke funnet.", id);
                return NotFound(new ApiError { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feil ved oppdatering av bruker med ID {Id}", id);
                return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _userService.DeleteUser(id);
                return Ok(new { message = $"Bruker med ID {id} er slettet." });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Bruker med ID {Id} ikke funnet for sletting.", id);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feil ved sletting av bruker med ID {Id}", id);
                return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
            }
        }

    }
}