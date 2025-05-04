using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvApi2.Migrations
{
    /// <inheritdoc />
    public partial class imagesAddedOnCv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Cvs",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Cvs");
        }
    }
}
