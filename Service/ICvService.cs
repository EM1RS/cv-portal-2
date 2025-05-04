using CvAPI2.Models;
using CvAPI2.Models.Tag;
using Microsoft.AspNetCore.Mvc;

public interface ICvService
{
    Task<List<CvDto>> GetAllCvDtos();
    Task<Cv?> GetCvById(string id);
    Task<CvDto?> GetCvDtoById(string id);
    Task<Cv?> GetCvForUser(string userId);
    Task<Cv> CreateCvFromDto(string userId, CreateCvDto dto);
    Task UpdateCvFromDto(Cv existingCv, UpdateCvDto dto);
    Task DeleteCv(string id);
    Task<List<CvDto>> SearchCvsByKeywords(List<string> keywords);
    Task<Tag> GetOrCreateTagAsync(string value);
    Task UpdateMyCv(Cv existingCv, UpdateCvDto dto);
    Task<CvProfileDto?> GetCvProfileForUser(string userId);
    //CvForAI MapCvDtoToCvForAI(CvDto dto);
    //Task<string> GetCvSummaryFromOpenAIAsync(CvForAI cv);



}