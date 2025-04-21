using CvAPI2.DTO;
using CvAPI2.Models;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsers();
    Task<UserDto> GetUserById(string id);
    Task<UserDto> CreateUser(CreateUserDto dto);
    Task UpdateUser(string id, UpdateUserDto dto);
    Task DeleteUser(string id);
}
