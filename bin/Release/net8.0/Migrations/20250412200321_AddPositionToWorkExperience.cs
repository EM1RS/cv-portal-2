using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvApi2.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionToWorkExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "WorkExperiences",
                newName: "Position");

            migrationBuilder.AlterColumn<string>(
                name: "CvId",
                table: "WorkExperiences",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CvId",
                table: "Educations",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Position",
                table: "WorkExperiences",
                newName: "Role");

            migrationBuilder.UpdateData(
                table: "WorkExperiences",
                keyColumn: "CvId",
                keyValue: null,
                column: "CvId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "CvId",
                table: "WorkExperiences",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Educations",
                keyColumn: "CvId",
                keyValue: null,
                column: "CvId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "CvId",
                table: "Educations",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
