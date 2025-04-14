using CvAPI2.Models;
using Microsoft.AspNetCore.Mvc;


public class CvService : ICvService

{
    private readonly ICvRepository _CvRepository;

    public CvService(ICvRepository CvRepository)
    {
        _CvRepository = CvRepository;
    }

    public Task<IEnumerable<Cv>> GetAllCvs() => _CvRepository.GetAllCvs();

    public Task<Cv?> GetCvById(string id) => _CvRepository.GetCvById(id);

    public async Task<Cv?> GetCvForUser(string userId) 
    {
        return await _CvRepository.GetAllCvs()
            .ContinueWith(task =>
                task.Result.FirstOrDefault(c => c.UserId == userId));
    }

    public Task AddCv(Cv cv) => _CvRepository.AddCv(cv);

    public Task UpdateCv(Cv cv) => _CvRepository.UpdateCv(cv);

    public async Task DeleteCv(string id) => await _CvRepository.DeleteCv(id);


    public async Task<List<Cv>> SearchCvsByKeywords(List<string> keywords)
    {
        return await _CvRepository.SearchCvsByKeywords(keywords);
    }

}