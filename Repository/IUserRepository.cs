using CvAPI2.Models;

public interface IUserRepository
{
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUserById(string id);
        Task CreateUser(User user);
        Task UpdateUser(string id, User user);
        Task DeleteUser(string id);

}