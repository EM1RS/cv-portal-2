using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvApi2.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSomClasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Skill_name",
                table: "CompetenceOverview",
                newName: "skill_name");

            migrationBuilder.RenameColumn(
                name: "Skill_level",
                table: "CompetenceOverview",
                newName: "skill_level");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "ProjectExperience",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "ProjectExperience",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "ProjectExperience");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "ProjectExperience");

            migrationBuilder.RenameColumn(
                name: "skill_name",
                table: "CompetenceOverview",
                newName: "Skill_name");

            migrationBuilder.RenameColumn(
                name: "skill_level",
                table: "CompetenceOverview",
                newName: "Skill_level");
        }
    }
}
