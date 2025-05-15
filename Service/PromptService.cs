using System.Text.Json;
using CvAPI2.Models;
using Microsoft.Extensions.Options;


public class PromptService : IPromptService
{
    private readonly string _openAiKey;
    private readonly ILogger<PromptService> _logger;
    private readonly ICvRepository _cvRepository;


    public PromptService(IOptions<OpenAiSettings> openAiSettings, ILogger<PromptService> logger, ICvRepository cvRepository)
    {
        _openAiKey = openAiSettings.Value.ApiKey;
        _logger = logger;
        _cvRepository = cvRepository;
    }

    public async Task<string> GenerateSummaryAsync(CvForAI cv)
    {
        try
        {
            var prompt = $@"
            Create a concise and professional summary based on this CV.
            Do not include any personal information. Focus on education, work experience, projects, skills, languages, and tags.
            The summary should be divided into two parts:
                1. Summary of education and work experience.
                2. Summary of projects, courses, certifications, awards, and competence overview. 
                Jeg vil ha norsk oppsummering / output.

            CV-data:
            {JsonSerializer.Serialize(cv, new JsonSerializerOptions { WriteIndented = true })}
            ";

            var requestBody = new
            {
                model = "gpt-4-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Du er en profesjonell karriereveileder." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", _openAiKey);

            var response = await client.PostAsJsonAsync("https://eso-m9r4t9se-eastus2.cognitiveservices.azure.com/openai/deployments/gpt-4o/chat/completions?api-version=2025-01-01-preview", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("OpenAI-feil ved generering av sammendrag: {Error}", error);
                return "Det oppstod en feil under generering av sammendrag.";
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var summary = json
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return summary ?? "Kunne ikke hente sammendrag.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved generering av sammendrag fra OpenAI.");
            return "Uventet feil under generering av sammendrag.";
        }
    }

    public async Task<bool> SaveSummaryToDatabaseAsync(string cvId, string summary)
    {
        var cv = await _cvRepository.GetCvById(cvId);
        if (cv == null)
        {
            _logger.LogWarning("CV ikke funnet ved lagring: {CvId}", cvId);
            return false;
        }

        var cvSummary = new CvSummary
        {
            CvId = cvId,
            SummaryText = summary
        };

        await _cvRepository.AddSummaryAsync(cvSummary);
        return true;
    }



    public async Task<string> GenerateAndOptionallySaveSummaryAsync(string cvId, bool save)
    {
        var cv = await _cvRepository.GetCvById(cvId);
        if (cv == null)
        {
            _logger.LogWarning("Ingen CV funnet med ID: {cvId}", cvId);
            return null;
        }
        var cvForAI = MapCvToCvForAI(cv); 
        var summary = await GenerateSummaryAsync(cvForAI);

        if (save)
        {
            await SaveSummaryToDatabaseAsync(cvId, summary);
        }

        return summary;
    }


    private CvForAI MapCvToCvForAI(Cv cv) // mapping cv -> CvForAI
    {
        return new CvForAI
        {
            WorkExperiences = cv.WorkExperiences?.Select(w => new WorkExperienceDto
            {
                WorkExperienceDescription = w.WorkExperienceDescription,
                Company = w.CompanyName,
                Position = w.Position,
                From = w.StartDate,
                To = w.EndDate ?? default,
               Tags = w.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
            }).ToList() ?? new List<WorkExperienceDto>(),


            ProjectExperiences = cv.ProjectExperiences?.Select(p => new ProjectExperienceDto
            {
                ProjectName = p.ProjectName,
                CompanyName = p.CompanyName,
                ProjectExperienceDescription = p.ProjectExperienceDescription,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Tags = p.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
            }).ToList() ?? new List<ProjectExperienceDto>(),


            Educations = cv.Educations?.Select(e => new EducationDto
            {
                Degree = e.Degree,
                School = e.School,
                EducationDescription = e.EducationDescription,
                StartYear = e.StartDate,
                EndYear = e.EndDate
            }).ToList() ?? new List<EducationDto>(),

            Courses = cv.Courses?.Select(c => new CourseDto
            {
                Name = c.Name,
                CourseDescription = c.CourseDescription,
                Provider = c.Provider,
                CompletionDate = c.Date
            }).ToList() ?? new List<CourseDto>(),

            Certifications = cv.Certifications?.Select(c => new CertificationDto
            {
                Name = c.Name,
                CertificationDescription = c.CertificationDescription,
                IssuedBy = c.IssuedBy,
                Date = c.Date
            }).ToList() ?? new List<CertificationDto>(),

            Awards = cv.Awards?.Select(a => new AwardDto
            {
                Name = a.Name,
                Organization = a.Organization,
                AwardDescription = a.AwardDescription,
                Year = a.Year
            }).ToList() ?? new List<AwardDto>(),

            CompetenceOverviews = cv.CompetenceOverviews?.Select(co => new CompetenceOverviewDto
            {
                Area = co.skill_name,
                Level = co.skill_level,
                
            }).ToList() ?? new List<CompetenceOverviewDto>(),

            Languages = cv.Languages?.Select(l => new LanguageDto
            {
                Name = l.Name,
                Proficiency = l.Proficiency
            }).ToList() ?? new List<LanguageDto>(),

            RoleOverviews = cv.RoleOverviews?.Select(r => new RoleOverviewDto
            {
                Role = r.Role,
                RoleDescription = r.RoleDescription
            }).ToList() ?? new List<RoleOverviewDto>(),

        };
    }
    public async Task<CvSummary?> GetSummaryByIdAsync(string cvId)
    {
        return await _cvRepository.GetSummaryByIdAsync(cvId);
    }


    public async Task DeleteSummaryAsync(string summaryId)
    {
        await _cvRepository.DeleteSummaryAsync(summaryId);
    }

    public async Task<string> EvaluateCandidateAsync(string cvId, string requirements)
    {
        var cv = await _cvRepository.GetCvById(cvId);
            if (cv == null)
            {
                _logger.LogWarning("Ingen CV funnet med ID {CvId} ved evaluering", cvId);
                return null;
            }

        var cvForAi = MapCvToCvForAI(cv);

        var prompt = $@"
            Here is a list of competency requirements, followed by a CV text.
            Jeg har følgende krav:
            {requirements}

            For each requirement:
            - Read the CV text carefully.
            - Assess whether the requirement is fulfilled based on the information in the CV.
            - Write the answer directly below each requirement.

            For hver krav i listen:
            - Les gjennom CV-teksten nøye.
            - Vurder om kravet er oppfylt basert på informasjonen i CV-en.
            - Skriv svaret rett under hvert krav.

            Answer format:
            - Start each answer with: 'Svar: Ja', 'Svar: Nei' or 'Svar: Delvis'
            - Provide a short explanation (1–2 sentences)
            - If the answer is based on a specific job or project, you MUST include the period in this format: (YYYY–YYYY)
            - Do NOT add summaries, headings or any extra text
            - Respond in the exact same order as the requirements are listed
            - The response MUST be written in Norwegian

            Eksempel:
            React.js  
            Svar: Ja, brukt i prosjekt for Helsedirektoratet (2016–2017) hos Bouvet som Frontend-utvikler.

            Bare krav -> svar -> krav -> svar, slik at det kan kopieres rett inn i en tabell.

            Kandidat:
            {cvForAi.GetSummary()}
            ";

        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "Du er en profesjonell rekrutterer." },
                new { role = "user", content = prompt }
            },
            temperature = 0
        };

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("api-key", _openAiKey);

        var response = await client.PostAsJsonAsync("https://eso-m9r4t9se-eastus2.cognitiveservices.azure.com/openai/deployments/gpt-4o/chat/completions?api-version=2025-01-01-preview", requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("OpenAI-feil ved evaluering: {Error}", error);
            return null;
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var evaluationResult = json
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return evaluationResult ?? "Ukjent svar.";
    }


}

