using CvAPI2.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserRepository : IUserRepository
{
    private readonly CvDbContext _context;

    public UserRepository(CvDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _context.Users
            .Include(u => u.Cv)
                .ThenInclude(c => c.WorkExperiences)
            .Include(u => u.Cv)
                .ThenInclude(c => c.Educations)
            .Include(u => u.Role)
            .ToListAsync();
    }

    public async Task<User> GetUserById(string id)
    {
        var user = await _context.Users
            .Include(u => u.Cv)
                .ThenInclude(c => c.WorkExperiences)
            .Include(u => u.Cv)
                .ThenInclude(c => c.Educations)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return user;
    }

    public async Task AddUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUser(string id, User user)
    {
        var existingUser = await _context.Users
            .Include(u => u.Cv)
                .ThenInclude(c => c.WorkExperiences)
            .Include(u => u.Cv)
                .ThenInclude(c => c.Educations)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (existingUser == null)
        {
            throw new NotFoundException("User not found");
        }

        existingUser.FullName = user.FullName;
        existingUser.Email = user.Email;
        existingUser.PhoneNumber = user.PhoneNumber;
        existingUser.Cv.WorkExperiences = user.Cv.WorkExperiences;
        existingUser.Cv.Educations = user.Cv.Educations;
        existingUser.RoleId = user.RoleId;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(string id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
