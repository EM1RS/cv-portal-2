using CvAPI2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsers();  
    Task<User> GetUserById(string id);  
    Task AddUser(User user);            
    Task UpdateUser(string id, User user);  
    Task DeleteUser(string id);            
}