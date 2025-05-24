using CvAPI2.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class CvPdfDocument : IDocument
{
    private readonly Cv _cv;
    private readonly CvPdfOptions _options;
    private readonly byte[]? _profileImageBytes;

    public CvPdfDocument(Cv cv, CvPdfOptions options, byte[]? profileImageBytes = null)
    {
        _cv = cv;
        _options = options;
        _profileImageBytes = profileImageBytes;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.Size(PageSizes.A4);

            page.Content().PaddingVertical(10).Column(col =>
            {
                col.Spacing(15);

                
                col.Item().Row(row =>
                {
                    if (_profileImageBytes != null)
                    {
                        row.ConstantItem(60).Element(img =>
                        {
                            img.Height(60)
                            .Width(60)
                            .Image(_profileImageBytes)
                            .FitArea();
                        });
                    }

                    row.RelativeItem().AlignMiddle().Column(text =>
                    {
                        text.Spacing(4);
                        text.Item().Text($"CV – {_cv.User?.FullName ?? "Ukjent"}")
                            .FontSize(24).Bold().FontColor(Colors.Blue.Medium);

                        text.Item().Text($"E-post: {_cv.User?.Email ?? "-"}")
                            .FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
                    });
                });


                // Personlig tekst
                col.Item().Text(_cv.Personalia).FontSize(12);

                // Seksjoner
                if (_options.IncludeEducations) col.Item().Element(ComposeEducation);
                if (_options.WorkExperienceIds?.Any() == true) col.Item().Element(ComposeWorkExperience);
                if (_options.ProjectExperienceIds?.Any() == true) col.Item().Element(ComposeProjectExperiences);
                if (_options.IncludeLanguages) col.Item().Element(ComposeLanguages);
                if (_options.IncludeRoleOverviews) col.Item().Element(ComposeRoleOverviews);
                if (_options.IncludeCourses) col.Item().Element(ComposeCourses);
                if (_options.IncludeCertifications) col.Item().Element(ComposeCertifications);
                if (_options.IncludeCompetenceOverviews) col.Item().Element(ComposeCompetenceOverviews);
                if (_options.IncludeAwards) col.Item().Element(ComposeAwards);
            });

            // Footer
            page.Footer().AlignCenter().Text("Generert av KickStep CV Portal ©")
                .FontSize(9).FontColor(Colors.Grey.Medium);
        });
    }

    void ComposeEducation(IContainer container) => container.Column(col =>
    {
        col.Item().Text("Educations").FontSize(16).Bold().Underline();
        foreach (var edu in _cv.Educations)
            col.Item().Text($"{edu.Degree} ved {edu.School} ({edu.StartDate:yyyy} - {edu.EndDate:yyyy})").FontSize(12);
    });

    void ComposeWorkExperience(IContainer container) => container.Column(col =>
    {
        col.Item().Text("WorkExperiences").FontSize(16).Bold().Underline();
        foreach (var job in _cv.WorkExperiences.Where(w => _options.WorkExperienceIds.Contains(w.Id)))
        {
            var period = $"{job.StartDate:yyyy} - {job.EndDate?.ToString("yyyy") ?? "nå"}";
            col.Item().Text($"{job.Position} hos {job.CompanyName} ({period})").FontSize(12).Bold();
            col.Item().Text(job.WorkExperienceDescription).FontSize(11);
            if (job.Tags.Any())
                col.Item().Text("Tags: " + string.Join(", ", job.Tags.Select(t => t.Tag.Value))).FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
        }
    });

    void ComposeProjectExperiences(IContainer container) => container.Column(col =>
    {
        col.Item().Text("ProjectExperiences").FontSize(16).Bold().Underline();
        foreach (var proj in _cv.ProjectExperiences.Where(p => _options.ProjectExperienceIds.Contains(p.Id)))
        {
            var period = $"{proj.StartDate:yyyy} - {proj.EndDate.ToString("yyyy") ?? "nå"}";
            col.Item().Text($"{proj.ProjectName} ({period})").FontSize(12).Bold();
            col.Item().Text(proj.ProjectExperienceDescription).FontSize(11);
            if (proj.Tags.Any())
                col.Item().Text("Tags: " + string.Join(", ", proj.Tags.Select(t => t.Tag.Value))).FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
        }
    });

    void ComposeLanguages(IContainer container) => container.Column(col =>
    {
        col.Item().Text("Languages").FontSize(16).Bold().Underline();
        foreach (var lang in _cv.Languages)
            col.Item().Text($"{lang.Name} – {lang.Proficiency}").FontSize(11);
    });

    void ComposeRoleOverviews(IContainer container) => container.Column(col =>
    {
        col.Item().Text("RoleOverviews").FontSize(16).Bold().Underline();
        foreach (var role in _cv.RoleOverviews)
            col.Item().Text($"{role.Role}: {role.RoleDescription}").FontSize(11);
    });

    void ComposeCourses(IContainer container) => container.Column(col =>
    {
        col.Item().Text("Courses").FontSize(16).Bold().Underline();
        foreach (var c in _cv.Courses)
            col.Item().Text($"{c.Name} ({c.Provider}, {c.Date:yyyy}): {c.CourseDescription}").FontSize(11);
    });

    void ComposeCertifications(IContainer container) => container.Column(col =>
    {
        col.Item().Text("Certifications").FontSize(16).Bold().Underline();
        foreach (var c in _cv.Certifications)
            col.Item().Text($"{c.Name} fra {c.IssuedBy} ({c.Date:yyyy}): {c.CertificationDescription}").FontSize(11);
    });

    void ComposeCompetenceOverviews(IContainer container) => container.Column(col =>
    {
        col.Item().Text("CompetenceOverviews").FontSize(16).Bold().Underline();
        foreach (var c in _cv.CompetenceOverviews)
            col.Item().Text($"{c.skill_name}: {c.skill_level}").FontSize(11);
    });

    void ComposeAwards(IContainer container) => container.Column(col =>
    {
        col.Item().Text("Awards").FontSize(16).Bold().Underline();
        foreach (var a in _cv.Awards)
            col.Item().Text($"{a.Name} – {a.Organization} ({a.Year}): {a.AwardDescription}").FontSize(11);
    });
}
