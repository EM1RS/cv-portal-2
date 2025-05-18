using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using CvAPI2.Models; // Endre til din faktiske namespace
using CvAPI2.Services; // Der du henter ut CV-data
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class CvPdfController : ControllerBase
{
    private readonly ICvService _cvService;

    public CvPdfController(ICvService cvService)
    {
        _cvService = cvService;
    }

    [HttpPost("{cvId}")]
    public async Task<IActionResult> GetPdf(string cvId, [FromBody] CvPdfOptions request)
    {
        var cv = await _cvService.GetCvById(cvId);
        if (cv == null)
            return NotFound("CV not found");

        // Filter experiences based on selected IDs
        if (request.WorkExperienceIds?.Any() == true)
        {
            cv.WorkExperiences = cv.WorkExperiences
                .Where(w => request.WorkExperienceIds.Contains(w.Id))
                .ToList();
        }
        else
        {
            cv.WorkExperiences = new List<WorkExperience>();
        }

        if (request.ProjectExperienceIds?.Any() == true)
        {
            cv.ProjectExperiences = cv.ProjectExperiences
                .Where(p => request.ProjectExperienceIds.Contains(p.Id))
                .ToList();
        }
        else
        {
            cv.ProjectExperiences = new List<ProjectExperience>();
        }

        // Lag ny options, resten kommer som bools
        var options = new CvPdfOptions
        {
            IncludeEducations = request.IncludeEducations,
            IncludeLanguages = request.IncludeLanguages,
            IncludeRoleOverviews = request.IncludeRoleOverviews,
            IncludeCourses = request.IncludeCourses,
            IncludeCertifications = request.IncludeCertifications,
            IncludeCompetenceOverviews = request.IncludeCompetenceOverviews,
            IncludeAwards = request.IncludeAwards,

            // Disse sender vi med som tom liste (vi har allerede filtrert over)
            WorkExperienceIds = request.WorkExperienceIds,
            ProjectExperienceIds = request.ProjectExperienceIds
        };

        var document = new CvPdfDocument(cv, options);
        var pdfBytes = document.GeneratePdf();

        return File(pdfBytes, "application/pdf", $"CV_{cv.User?.FullName ?? "user"}.pdf");
    }

}
