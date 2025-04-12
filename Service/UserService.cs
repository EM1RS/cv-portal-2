using CvAPI2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;  // Husk å importere denne for asynkrone metoder

namespace CvAPI2.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _userRepository.GetUsers();  
        }

        public async Task<User> GetUserById(string id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                throw new NotFoundException($"User with id {id} not found.");
            }
            return user;
        }

        public async Task AddUser(User user)
        {
            await _userRepository.AddUser(user);  
        }

        public async Task UpdateUser(string id, User user)
        {
            var existingUser = await _userRepository.GetUserById(id);
            if (existingUser == null)
            {
                throw new NotFoundException($"User with id {id} not found.");
            }
            await _userRepository.UpdateUser(id, user);  
        }

        public async Task DeleteUser(string id)
        {
            var existingUser = await _userRepository.GetUserById(id);
            if (existingUser == null)
            {
                throw new NotFoundException($"User with id {id} not found.");
            }
            await _userRepository.DeleteUser(id);  
        }
    }
}
