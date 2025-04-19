using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvApi2.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkExperienceDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WorkExperiences",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Educations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Cvs",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Courses",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Certifications",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Awards",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "WorkExperiences");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Cvs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Awards");
        }
    }
}
