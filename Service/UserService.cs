using CvAPI2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CvAPI2.DTO;
using Microsoft.AspNetCore.Identity;


namespace CvAPI2.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public UserService(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "ingen";

                userDtos.Add(new UserDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = role,
                    CvId = user.Cv?.Id
                });
            }

            return userDtos;
        }


        public async Task<UserDto> GetUserById(string id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
                throw new NotFoundException($"User with id {id} not found.");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "ingen";

            return new UserDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = role,
                CvId = user.Cv?.Id
            };
        }


        public async Task<UserDto> CreateUser(CreateUserDto newUser)
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
                var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Opprettelse feilet: {errorMessages}");
            }

            if (!string.IsNullOrEmpty(newUser.Role))
            {
                await _userManager.AddToRoleAsync(user, newUser.Role);
            }

            return new UserDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = newUser.Role
            };
        }

        public async Task UpdateUser(string id, UpdateUserDto updatedUser)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException($"User with id {id} not found.");
            }

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.UserName = updatedUser.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Oppdatering feilet: {errorMessages}");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrEmpty(updatedUser.Role))
            {
                await _userManager.AddToRoleAsync(user, updatedUser.Role);
            }
        }

        public async Task DeleteUser(string id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
                throw new NotFoundException($"User with id {id} not found.");

            if (user.UserName.StartsWith("admin@", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Kan ikke slette admin-bruker.");

            await _userRepository.DeleteUser(id);
        }
        
    }
}
