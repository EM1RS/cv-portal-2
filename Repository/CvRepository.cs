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
    public async Task<List<Cv>> SearchCvsByKeywords(List<string> keywords)
    {
        if (keywords == null || keywords.Count == 0)
            return new List<Cv>();
        var loweredKeywords = keywords.Select(k => k.ToLower()).ToList();

        var cvs = await _context.Cvs
            .Include(c => c.User)
            .Include(c => c.WorkExperiences)
            .Include(c => c.Educations)
            .Include(c => c.CompetenceOverviews)
            .Include(c => c.Languages)
            .Include(c => c.Positions)
            .Include(c => c.ProjectExperiences)
            .Include(c => c.Certifications)
            .Include(c => c.Courses)
            .Include(c => c.Awards)
            .Include(c => c.RoleOverviews)
            .AsNoTracking()
            .ToListAsync();

    return cvs
            .Where(cv =>
                keywords.Any(k =>
                    (cv.Personalia?.ToLower().Contains(k) ?? false) ||
                    (cv.User?.FullName?.ToLower().Contains(k) ?? false) ||
                    (cv.Educations?.Any(e => e.School.ToLower().Contains(k) || e.Degree.ToLower().Contains(k)) ?? false) ||
                    (cv.WorkExperiences?.Any(w => w.Company.ToLower().Contains(k) || w.Position.ToLower().Contains(k)) ?? false) ||
                    (cv.Certifications?.Any(c => c.Name.ToLower().Contains(k) || c.IssuedBy.ToLower().Contains(k)) ?? false) ||
                    (cv.Courses?.Any(c => c.Name.ToLower().Contains(k) || c.Provider.ToLower().Contains(k)) ?? false) ||
                    (cv.CompetenceOverviews?.Any(c => c.skill_name.ToLower().Contains(k) || c.skill_level.ToLower().Contains(k)) ?? false) ||
                    (cv.Languages?.Any(l => l.Name.ToLower().Contains(k) || l.Proficiency.ToLower().Contains(k)) ?? false) ||
                    (cv.Positions?.Any(p => p.Name.ToLower().Contains(k)) ?? false) ||
                    (cv.ProjectExperiences?.Any(p => p.ProjectName.ToLower().Contains(k) || p.Description.ToLower().Contains(k) || p.Role.ToLower().Contains(k)) ?? false) ||
                    (cv.RoleOverviews?.Any(r => r.Role.ToLower().Contains(k) || r.Description.ToLower().Contains(k)) ?? false) ||
                    (cv.Awards?.Any(a => a.Name.ToLower().Contains(k) || a.Organization.ToLower().Contains(k)) ?? false)
                )
            )
            .ToList();
                    
    }
}
