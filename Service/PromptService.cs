using System.Text;
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
            Lag en profesjonell og helhetlig tekstbasert oppsummering (maks 2 avsnitt). Start alltid med kandidatens prosjekt­erfaring. 
            Utdanning og arbeidserfaring nevnes kun som støtte. Unngå punktlister. Ikke inkluder personlige opplysninger. Prosjekt datoer er viktig.


            CV-data:
            {BuildCvSummaryForAI(cv)}
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
                CompanyName = w.CompanyName,
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
                StudyName = e.StudyName,
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

    public async Task<string> MatrixRequirementAsync(string cvId, string requirements)
    {
        var cv = await _cvRepository.GetCvById(cvId);
        if (cv == null)
        {
            _logger.LogWarning("Ingen CV funnet med ID {CvId} ved evaluering", cvId);
            return null;
        }

        var cvForAi = MapCvToCvForAI(cv);

        var prompt = $@"
        Her er en liste med kompetansekrav fra en kunde, etterfulgt av en CV.

        🟨 For hvert krav:
        - Bruk primært informasjon fra prosjektdelen i CV-en.
        - Dersom et prosjekt dekker kravet, MÅ svaret inneholde:
        • Prosjektets navn (hvis tilgjengelig)
        • Fra- og til-dato i format: (MM–YYYY til MM–YYYY)
        • En kort forklaring på hva teknologien/metoden ble brukt til i prosjektet.
        - Hvis det ikke finnes relevant prosjekt, bruk annen erfaring og merk det tydelig.

        📌 Format:
        1. Skriv kravet (på egen linje)
        2. Svar: Ja / Nei / Delvis
        3. Forklaring på 1–3 linjer: HVA ble gjort og HVORDAN teknologien/metoden ble brukt. Nevne prosjekt og periode.

        🛑 Ikke inkluder overskrifter, punktlister eller summeringer. Bare:  
        krav  
        Svar: …  
        Forklaring

        Krav:
        {requirements}

        CV:
        {cvForAi.GetSummary()}
        ";


        var requestBody = new
        {
            model = "gpt-4-turbo",
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

    private string BuildCvSummaryForAI(CvForAI cv)
    {
        var sb = new StringBuilder();

        // 1. Prosjekter (viktigst)
        sb.AppendLine("🧠 Prosjekter:");
        foreach (var p in cv.ProjectExperiences ?? new List<ProjectExperienceDto>())
        {
            var from = p.StartDate.Year.ToString();
            var to = p.EndDate.Year.ToString();
            var period = $"({from}–{to})";
            sb.AppendLine($"Prosjekt: {p.ProjectName} {period}");

            sb.AppendLine($"Prosjekt: {p.ProjectName} {period}");
            if (!string.IsNullOrWhiteSpace(p.Role))
                sb.AppendLine($"Rolle: {p.Role}");
            if (p.Tags?.Any() == true)
                sb.AppendLine($"Teknologier: {string.Join(", ", p.Tags)}");
            if (!string.IsNullOrWhiteSpace(p.ProjectExperienceDescription))
                sb.AppendLine($"Beskrivelse: {p.ProjectExperienceDescription}");
            sb.AppendLine();
        }

        // 2. Arbeidserfaring (støtte)
        sb.AppendLine("👔 Arbeidserfaring:");
        foreach (var w in cv.WorkExperiences ?? new List<WorkExperienceDto>())
        {
            var from = w.From.Year.ToString() ?? "?";
            var to = w.To?.Year.ToString() ?? "?";
            var period = $"({from}–{to})";
            

            sb.AppendLine($"Stilling: {w.Position} hos {w.CompanyName} {period}");
            if (!string.IsNullOrWhiteSpace(w.WorkExperienceDescription))
                sb.AppendLine($"Beskrivelse: {w.WorkExperienceDescription}");
            if (w.Tags?.Any() == true)
                sb.AppendLine($"Ferdigheter: {string.Join(", ", w.Tags)}");
            sb.AppendLine();
        }

        // 3. Utdanning
        sb.AppendLine("🎓 Utdanning:");
        foreach (var e in cv.Educations ?? new List<EducationDto>())
        {
            var from = e.StartYear.Year.ToString() ?? "?";
            var to = e.EndYear.Year.ToString() ?? "?";
            var period = $"({from}–{to})";

            sb.AppendLine($"Studium: {e.Degree} ved {e.School} {period}");
            if (!string.IsNullOrWhiteSpace(e.EducationDescription))
                sb.AppendLine($"Beskrivelse: {e.EducationDescription}");
            sb.AppendLine();
        }

        return sb.ToString();
    }



}

