using CvAPI2.Models;
using Microsoft.EntityFrameworkCore;

public class CvRepository : ICvRepository
{
    private readonly CvDbContext _context;

    public CvRepository(CvDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cv>> GetAllCvs() =>
        await _context.Cvs
            .Include(c => c.User)
            .Include(c => c.WorkExperiences)
            .Include(c => c.Educations)
            .Include(c => c.Awards)
            .Include(c => c.Courses)
            .Include(c => c.Certifications)
            .Include(c => c.CompetenceOverviews)
            .Include(c => c.RoleOverviews)
            .Include(c => c.ProjectExperiences)
            .Include(c => c.Positions)
            .Include(c => c.Languages)
            .ToListAsync();

    public async Task<Cv?> GetCvById(string id) =>
        await _context.Cvs
            .Include(c => c.User)
            .Include(c => c.WorkExperiences)
            .Include(c => c.Educations)
            .Include(c => c.Awards)
            .Include(c => c.Courses)
            .Include(c => c.Certifications)
            .Include(c => c.CompetenceOverviews)
            .Include(c => c.RoleOverviews)
            .Include(c => c.ProjectExperiences)
            .Include(c => c.Positions)
            .Include(c => c.Languages)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Cv>> GetCvByUserId(string userId) =>
        await _context.Cvs
            .Where(cv => cv.UserId == userId)
            .Include(c => c.User)
            .Include(c => c.WorkExperiences)
            .Include(c => c.Educations)
            .Include(c => c.Awards)
            .Include(c => c.Courses)
            .Include(c => c.Certifications)
            .Include(c => c.CompetenceOverviews)
            .Include(c => c.RoleOverviews)
            .Include(c => c.ProjectExperiences)
            .Include(c => c.Positions)
            .Include(c => c.Languages)
            .ToListAsync();

    public async Task AddCv(Cv cv)
    {
        _context.Cvs.Add(cv);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCv(Cv cv)
    {
        _context.Cvs.Update(cv);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCv(string id)
    {
        var cv = await _context.Cvs.FindAsync(id);
        if (cv != null)
        {
            _context.Cvs.Remove(cv);
            await _context.SaveChangesAsync();
        }
    }
}
