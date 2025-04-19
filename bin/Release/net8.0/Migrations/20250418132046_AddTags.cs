using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvApi2.Migrations
{
    /// <inheritdoc />
    public partial class AddTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Award_Cvs_CvId",
                table: "Award");

            migrationBuilder.DropForeignKey(
                name: "FK_Certification_Cvs_CvId",
                table: "Certification");

            migrationBuilder.DropForeignKey(
                name: "FK_CompetenceOverview_Cvs_CvId",
                table: "CompetenceOverview");

            migrationBuilder.DropForeignKey(
                name: "FK_Course_Cvs_CvId",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_Language_Cvs_CvId",
                table: "Language");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectExperience_Cvs_CvId",
                table: "ProjectExperience");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleOverview_Cvs_CvId",
                table: "RoleOverview");

            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleOverview",
                table: "RoleOverview");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectExperience",
                table: "ProjectExperience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Language",
                table: "Language");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Course",
                table: "Course");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompetenceOverview",
                table: "CompetenceOverview");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certification",
                table: "Certification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Award",
                table: "Award");

            migrationBuilder.RenameTable(
                name: "RoleOverview",
                newName: "RoleOverviews");

            migrationBuilder.RenameTable(
                name: "ProjectExperience",
                newName: "ProjectExperiences");

            migrationBuilder.RenameTable(
                name: "Language",
                newName: "Languages");

            migrationBuilder.RenameTable(
                name: "Course",
                newName: "Courses");

            migrationBuilder.RenameTable(
                name: "CompetenceOverview",
                newName: "CompetenceOverviews");

            migrationBuilder.RenameTable(
                name: "Certification",
                newName: "Certifications");

            migrationBuilder.RenameTable(
                name: "Award",
                newName: "Awards");

            migrationBuilder.RenameColumn(
                name: "Company",
                table: "WorkExperiences",
                newName: "CompanyName");

            migrationBuilder.RenameIndex(
                name: "IX_RoleOverview_CvId",
                table: "RoleOverviews",
                newName: "IX_RoleOverviews_CvId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectExperience_CvId",
                table: "ProjectExperiences",
                newName: "IX_ProjectExperiences_CvId");

            migrationBuilder.RenameIndex(
                name: "IX_Language_CvId",
                table: "Languages",
                newName: "IX_Languages_CvId");

            migrationBuilder.RenameIndex(
                name: "IX_Course_CvId",
                table: "Courses",
                newName: "IX_Courses_CvId");

            migrationBuilder.RenameIndex(
                name: "IX_CompetenceOverview_CvId",
                table: "CompetenceOverviews",
                newName: "IX_CompetenceOverviews_CvId");

            migrationBuilder.RenameIndex(
                name: "IX_Certification_CvId",
                table: "Certifications",
                newName: "IX_Certifications_CvId");

            migrationBuilder.RenameIndex(
                name: "IX_Award_CvId",
                table: "Awards",
                newName: "IX_Awards_CvId");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "ProjectExperiences",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleOverviews",
                table: "RoleOverviews",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectExperiences",
                table: "ProjectExperiences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Languages",
                table: "Languages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompetenceOverviews",
                table: "CompetenceOverviews",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certifications",
                table: "Certifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Awards",
                table: "Awards",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProjectExperienceTags",
                columns: table => new
                {
                    ProjectExperienceId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectExperienceTags", x => new { x.ProjectExperienceId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ProjectExperienceTags_ProjectExperiences_ProjectExperienceId",
                        column: x => x.ProjectExperienceId,
                        principalTable: "ProjectExperiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectExperienceTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkExperienceTags",
                columns: table => new
                {
                    WorkExperienceId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkExperienceTags", x => new { x.WorkExperienceId, x.TagId });
                    table.ForeignKey(
                        name: "FK_WorkExperienceTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkExperienceTags_WorkExperiences_WorkExperienceId",
                        column: x => x.WorkExperienceId,
                        principalTable: "WorkExperiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExperienceTags_TagId",
                table: "ProjectExperienceTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperienceTags_TagId",
                table: "WorkExperienceTags",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Awards_Cvs_CvId",
                table: "Awards",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_Cvs_CvId",
                table: "Certifications",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompetenceOverviews_Cvs_CvId",
                table: "CompetenceOverviews",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Cvs_CvId",
                table: "Courses",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Cvs_CvId",
                table: "Languages",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectExperiences_Cvs_CvId",
                table: "ProjectExperiences",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleOverviews_Cvs_CvId",
                table: "RoleOverviews",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Awards_Cvs_CvId",
                table: "Awards");

            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_Cvs_CvId",
                table: "Certifications");

            migrationBuilder.DropForeignKey(
                name: "FK_CompetenceOverviews_Cvs_CvId",
                table: "CompetenceOverviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Cvs_CvId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Cvs_CvId",
                table: "Languages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectExperiences_Cvs_CvId",
                table: "ProjectExperiences");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleOverviews_Cvs_CvId",
                table: "RoleOverviews");

            migrationBuilder.DropTable(
                name: "ProjectExperienceTags");

            migrationBuilder.DropTable(
                name: "WorkExperienceTags");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleOverviews",
                table: "RoleOverviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectExperiences",
                table: "ProjectExperiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Languages",
                table: "Languages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompetenceOverviews",
                table: "CompetenceOverviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certifications",
                table: "Certifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Awards",
                table: "Awards");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "ProjectExperiences");

            migrationBuilder.RenameTable(
                name: "RoleOverviews",
                newName: "RoleOverview");

            migrationBuilder.RenameTable(
                name: "ProjectExperiences",
                newName: "ProjectExperience");

            migrationBuilder.RenameTable(
                name: "Languages",
                newName: "Language");

            migrationBuilder.RenameTable(
                name: "Courses",
                newName: "Course");

            migrationBuilder.RenameTable(
                name: "CompetenceOverviews",
                newName: "CompetenceOverview");

            migrationBuilder.RenameTable(
                name: "Certifications",
                newName: "Certification");

            migrationBuilder.RenameTable(
                name: "Awards",
                newName: "Award");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "WorkExperiences",
                newName: "Company");

            migrationBuilder.RenameIndex(
                name: "IX_RoleOverviews_CvId",
                table: "RoleOverview",
                newName: "IX_RoleOverview_CvId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectExperiences_CvId",
                table: "ProjectExperience",
                newName: "IX_ProjectExperience_CvId");

            migrationBuilder.RenameIndex(
                name: "IX_Languages_CvId",
                table: "Language",
                newName: "IX_Language_CvId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_CvId",
                table: "Course",
                newName: "IX_Course_CvId");

            migrationBuilder.RenameIndex(
                name: "IX_CompetenceOverviews_CvId",
                table: "CompetenceOverview",
                newName: "IX_CompetenceOverview_CvId");

            migrationBuilder.RenameIndex(
                name: "IX_Certifications_CvId",
                table: "Certification",
                newName: "IX_Certification_CvId");

            migrationBuilder.RenameIndex(
                name: "IX_Awards_CvId",
                table: "Award",
                newName: "IX_Award_CvId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleOverview",
                table: "RoleOverview",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectExperience",
                table: "ProjectExperience",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Language",
                table: "Language",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Course",
                table: "Course",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompetenceOverview",
                table: "CompetenceOverview",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certification",
                table: "Certification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Award",
                table: "Award",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CvId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Organization = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Position_Cvs_CvId",
                        column: x => x.CvId,
                        principalTable: "Cvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Position_CvId",
                table: "Position",
                column: "CvId");

            migrationBuilder.AddForeignKey(
                name: "FK_Award_Cvs_CvId",
                table: "Award",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certification_Cvs_CvId",
                table: "Certification",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompetenceOverview_Cvs_CvId",
                table: "CompetenceOverview",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Cvs_CvId",
                table: "Course",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Language_Cvs_CvId",
                table: "Language",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectExperience_Cvs_CvId",
                table: "ProjectExperience",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleOverview_Cvs_CvId",
                table: "RoleOverview",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
