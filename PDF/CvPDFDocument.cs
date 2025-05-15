using CvAPI2.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class CvPdfDocument : IDocument
{
    private readonly Cv _cv;

    public CvPdfDocument(Cv cv)
    {
        _cv = cv;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.Header().Text($"CV – {_cv.User?.FullName ?? "Ukjent"}").FontSize(20).Bold();
            page.Content().Column(col =>
            {
                col.Spacing(10);

                col.Item().Text($"E-post: {_cv.User?.Email}");
                col.Item().Text($"Oppsummering: {_cv.Personalia}");

                col.Item().Element(ComposeEducation);
                col.Item().Element(ComposeWorkExperience);
                col.Item().Element(ComposeLanguages);
                col.Item().Element(ComposeRoleOverviews);
                col.Item().Element(ComposeCourses);
                col.Item().Element(ComposeCertifications);
                col.Item().Element(ComposeProjectExperiences);
                col.Item().Element(ComposeCompetenceOverviews);
                col.Item().Element(ComposeAwards);
            });
        });
    }

    void ComposeEducation(IContainer container) =>
        container.Column(col =>
        {
            col.Item().Text("Utdanning").FontSize(14).Bold();
            foreach (var edu in _cv.Educations)
                col.Item().Text($"• {edu.Degree} ved {edu.School} ({edu.StartDate:yyyy} - {edu.EndDate.ToString("yyyy") ?? "pågår"})");
        });

    void ComposeWorkExperience(IContainer container) =>
        container.Column(col =>
        {
            col.Item().Text("Arbeidserfaring").FontSize(14).Bold();

            foreach (var job in _cv.WorkExperiences)
            {
                var endYear = job.EndDate?.ToString("yyyy") ?? "pågår";
                col.Item().Text($"• {job.Position} hos {job.CompanyName} ({job.StartDate:yyyy} - {endYear})");

                foreach (var tag in job.Tags.Select(t => t.Tag.Value))
                {
                    col.Item().Text($"   – Tag: {tag}");
                }
            }
        });

    void ComposeProjectExperiences(IContainer container) =>
        container.Column(col =>
        {
            col.Item().Text("Prosjekter").FontSize(14).Bold();
            foreach (var proj in _cv.ProjectExperiences)
            {
                col.Item().Text($"• {proj.ProjectName} - {proj.CompanyName} ({proj.StartDate:yyyy} - {proj.EndDate.ToString("yyyy") ?? "pågår"})");
                foreach (var tag in proj.Tags.Select(t => t.Tag.Value))
                    {
                        col.Item().Text($"   – Tag: {tag}"); 
                    }
            }
        });


    void ComposeLanguages(IContainer container) =>
        container.Column(col =>
        {
            col.Item().Text("Språk").FontSize(14).Bold();
            foreach (var lang in _cv.Languages)
                col.Item().Text($"• {lang.Name} ({lang.Proficiency})");
        });

    void ComposeRoleOverviews(IContainer container) =>
        container.Column(col =>
        {
            col.Item().Text("Rolleoversikter").FontSize(14).Bold();
            foreach (var role in _cv.RoleOverviews)
                col.Item().Text($"• {role.Role}: {role.RoleDescription}");
        });

    void ComposeCourses(IContainer container) =>
        container.Column(col =>
        {
            col.Item().Text("Kurs").FontSize(14).Bold();
            foreach (var course in _cv.Courses)
                col.Item().Text($"• {course.Name}: ({course.CourseDescription}): ({course.Provider}, {course.Date:yyyy})");
        });

    void ComposeCertifications(IContainer container) =>
        container.Column(col =>
        {
            col.Item().Text("Sertifiseringer").FontSize(14).Bold();
            foreach (var cert in _cv.Certifications)
                col.Item().Text($"• {cert.Name} by {cert.IssuedBy}, {cert.CertificationDescription} - {cert.Date:yyyy}");
        });

    void ComposeCompetenceOverviews(IContainer container) =>
        container.Column(col =>
        {
            col.Item().Text("Kompetanseoversikter").FontSize(14).Bold();
            foreach (var comp in _cv.CompetenceOverviews)
                col.Item().Text($"• {comp.skill_name}: {comp.skill_level}");
        });

    void ComposeAwards(IContainer container) =>
        container.Column(col =>
        {
            col.Item().Text("Priser og utmerkelser").FontSize(14).Bold();
            foreach (var award in _cv.Awards)
                col.Item().Text($"• {award.Name}, {award.Organization}, {award.AwardDescription}, {award.Year}");
        });
}
