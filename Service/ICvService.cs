using CvAPI2.Models;
using Microsoft.AspNetCore.Mvc;

public interface ICvService
{
    Task<IEnumerable<Cv>> GetAllCvs();
    Task<Cv?> GetCvById(string id);
    Task<Cv?> GetCvForUser(string userId);
    Task AddCv(Cv cv);
    Task UpdateCv(Cv cv);
    Task DeleteCv(string id);
    Task<List<Cv>> SearchCvsByKeywords(List<string> keywords);
}