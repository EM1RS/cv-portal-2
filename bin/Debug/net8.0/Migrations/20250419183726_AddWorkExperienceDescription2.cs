using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvApi2.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkExperienceDescription2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "WorkExperiences",
                newName: "WorkExperienceDescription");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "RoleOverviews",
                newName: "RoleDescription");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ProjectExperiences",
                newName: "ProjectExperienceDescription");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Educations",
                newName: "EducationDescription");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Courses",
                newName: "CourseDescription");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Certifications",
                newName: "CertificationDescription");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Awards",
                newName: "AwardDescription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkExperienceDescription",
                table: "WorkExperiences",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "RoleDescription",
                table: "RoleOverviews",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ProjectExperienceDescription",
                table: "ProjectExperiences",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "EducationDescription",
                table: "Educations",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "CourseDescription",
                table: "Courses",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "CertificationDescription",
                table: "Certifications",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "AwardDescription",
                table: "Awards",
                newName: "Description");
        }
    }
}
