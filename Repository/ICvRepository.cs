using CvAPI2.Models;
using CvAPI2.Models.Tag;

public interface ICvRepository
{
    Task<IEnumerable<Cv>> GetAllCvs();
    Task<Cv> GetCvById(string id);
    Task<IEnumerable<Cv>>GetCvByUserId(string userId);
    Task AddCv(Cv cv);
    Task UpdateCv(Cv cv);
    Task DeleteCv(string id);
    Task<List<Cv>> SearchCvsByKeywords(List<string> keywords);
    Task<Tag> GetOrCreateTagAsync(string value);

}