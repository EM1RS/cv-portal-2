
namespace CvAPI2.Models
{
    public interface IPromptService
    {
        Task<string> GenerateSummaryAsync(CvForAI cv);
        Task<bool> SaveSummaryToDatabaseAsync(string cvId, string summary); 
        Task<string?> GenerateAndOptionallySaveSummaryAsync(string cvId, bool save);
        Task<CvSummary?> GetSummaryByIdAsync(string cvId);
        Task DeleteSummaryAsync(string summaryId);
        Task<string?> EvaluateCandidateAsync(string cvId, string requirements);
    }

}