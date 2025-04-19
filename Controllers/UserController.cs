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
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, UserManager<User> userManager, ILogger<UsersController> logger)
        {
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetUsers();
                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault() ?? "ingen";

                    userDtos.Add(new UserDto
                    {
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = role,
                        UserId = user.Id,
                        CvId = user.Cv?.Id
                    });
                }

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
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                if (user == null)
                    return NotFound("Bruker ikke funnet.");

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "ingen";

                var userDto = new UserDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = role,
                    CvId = user.Cv?.Id
                };

                return Ok(userDto);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Bruker med ID {Id} ble ikke funnet.", id);
                return NotFound(ex.Message);
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
                var user = new User
                {
                    FullName = newUser.FullName,
                    Email = newUser.Email,
                    UserName = newUser.Email
                };

                var result = await _userManager.CreateAsync(user, newUser.Password);
                if (!result.Succeeded)
                {
                    var identityErrors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { message = "Feil under opprettelse", errors = identityErrors });
                }

                if (!string.IsNullOrEmpty(newUser.Role))
                {
                    await _userManager.AddToRoleAsync(user, newUser.Role);
                }

                var userDto = new UserDto
                {
                    Email = newUser.Email,
                    FullName = newUser.FullName
                };

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
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
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound("Bruker finnes ikke!");

                user.FullName = updatedUser.FullName;
                user.Email = updatedUser.Email;
                user.UserName = updatedUser.Email;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { message = "Oppdatering feilet!", errors });
                }

                var currentRole = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRole);

                if (!string.IsNullOrEmpty(updatedUser.Role))
                {
                    await _userManager.AddToRoleAsync(user, updatedUser.Role);
                }

                return NoContent();
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
                var user = await _userService.GetUserById(id);
                if (user == null) 
                    return NotFound("Bruker finnes ikke!");

                if (user.UserName.StartsWith("admin@", StringComparison.OrdinalIgnoreCase))
                { 
                    return BadRequest("Kan ikke slette admin-bruker.");
                }

                await _userService.DeleteUser(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Bruker med ID {Id} ikke funnet for sletting.", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feil ved sletting av bruker med ID {Id}", id);
                return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
            }
        }
    }
}