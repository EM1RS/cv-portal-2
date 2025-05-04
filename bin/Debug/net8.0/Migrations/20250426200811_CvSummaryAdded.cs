using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CvApi2.Migrations
{
    /// <inheritdoc />
    public partial class CvSummaryAdded : Migration
    {
        /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Først må du fjerne FOREIGN KEY constraint som bruker indexen
            migrationBuilder.DropForeignKey(
                name: "FK_CvSummaries_Cvs_CvId",
                table: "CvSummaries");

            // Nå kan du trygt fjerne indexen
            migrationBuilder.DropIndex(
                name: "IX_CvSummaries_CvId",
                table: "CvSummaries");

            // Opprett NY index (ikke unique)
            migrationBuilder.CreateIndex(
                name: "IX_CvSummaries_CvId",
                table: "CvSummaries",
                column: "CvId");

            // Legg tilbake en foreign key constraint (men nå tillater vi flere)
            migrationBuilder.AddForeignKey(
                name: "FK_CvSummaries_Cvs_CvId",
                table: "CvSummaries",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade); // eller Restrict hvis du vil
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CvSummaries_Cvs_CvId",
                table: "CvSummaries");

            migrationBuilder.DropIndex(
                name: "IX_CvSummaries_CvId",
                table: "CvSummaries");

            migrationBuilder.CreateIndex(
                name: "IX_CvSummaries_CvId",
                table: "CvSummaries",
                column: "CvId",
                unique: true); // Tilbake til én-til-én (hvis du ruller tilbake)
            
            migrationBuilder.AddForeignKey(
                name: "FK_CvSummaries_Cvs_CvId",
                table: "CvSummaries",
                column: "CvId",
                principalTable: "Cvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
