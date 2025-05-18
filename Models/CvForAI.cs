namespace CvAPI2.Models
{    
    public class CvForAI
    {
        public string UserId { get; set; }
        public List<EducationDto> Educations { get; set; }
        public List<WorkExperienceDto> WorkExperiences { get; set; }
        public List<ProjectExperienceDto> ProjectExperiences { get; set; }
        public List<CourseDto> Courses { get; set; }
        public List<CertificationDto> Certifications { get; set; }
        public List<AwardDto> Awards { get; set; }
        public List<CompetenceOverviewDto> CompetenceOverviews { get; set; }
        public List<LanguageDto> Languages { get; set; }
        public List<RoleOverviewDto> RoleOverviews { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

        public string GetSummary()
        {
            var summaryParts = new List<string>();

            // Erfaring
            if (WorkExperiences != null && WorkExperiences.Any())
            {
                var workExp = string.Join(", ", WorkExperiences.Select(w => w.Position + " hos " + w.CompanyName));
                summaryParts.Add($"Erfaring: {workExp}.");
            }

            // Prosjekter
            if (ProjectExperiences != null && ProjectExperiences.Any())
            {
                var projects = string.Join(", ", ProjectExperiences.Select(p => p.ProjectName));
                summaryParts.Add($"Prosjekter: {projects}.");
            }

            // Utdanning
            if (Educations != null && Educations.Any())
            {
                var education = string.Join(", ", Educations.Select(e => $"{e.Degree} ved {e.School}"));
                summaryParts.Add($"Utdanning: {education}.");
            }

            // Kurs
            if (Courses != null && Courses.Any())
            {
                var courses = string.Join(", ", Courses.Select(c => c.Name));
                summaryParts.Add($"Kurs: {courses}.");
            }

            // Sertifiseringer
            if (Certifications != null && Certifications.Any())
            {
                var certifications = string.Join(", ", Certifications.Select(c => c.Name));
                summaryParts.Add($"Sertifiseringer: {certifications}.");
            }

            // Priser/Awards
            if (Awards != null && Awards.Any())
            {
                var awards = string.Join(", ", Awards.Select(a => a.Name));
                summaryParts.Add($"Priser: {awards}.");
            }

            // Ferdigheter (competence overview)
            if (CompetenceOverviews != null && CompetenceOverviews.Any())
            {
                var skills = string.Join(", ", CompetenceOverviews.Select(c => c.Area));
                summaryParts.Add($"Ferdigheter: {skills}.");
            }

            // Språk
            if (Languages != null && Languages.Any())
            {
                var languages = string.Join(", ", Languages.Select(l => l.Name));
                summaryParts.Add($"Språk: {languages}.");
            }

            // Roller
            if (RoleOverviews != null && RoleOverviews.Any())
            {
                var roles = string.Join(", ", RoleOverviews.Select(r => r.Role));
                summaryParts.Add($"Roller: {roles}.");
            }

            // Tags (fra WorkExperience og ProjectExperience)
            var tags = new List<string>();

            if (WorkExperiences != null)
                tags.AddRange(WorkExperiences.SelectMany(w => w.Tags ?? new List<string>()));

            if (ProjectExperiences != null)
                tags.AddRange(ProjectExperiences.SelectMany(p => p.Tags ?? new List<string>()));

            if (tags.Any())
            {
                var tagsSummary = string.Join(", ", tags.Distinct());
                summaryParts.Add($"Teknologier: {tagsSummary}.");
            }

            // Kombiner alle deler
            return string.Join(" ", summaryParts);
        }


    }
}