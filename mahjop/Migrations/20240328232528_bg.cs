using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace mahjop.Migrations
{
    public partial class bg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HealthAssessments",
                columns: table => new
                {
                    HealthAssessmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BloodPressure = table.Column<double>(type: "float", nullable: false),
                    BloodSugarLevel = table.Column<double>(type: "float", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthAssessments", x => x.HealthAssessmentId);
                    table.ForeignKey(
                        name: "FK_HealthAssessments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthAssessments_PatientId",
                table: "HealthAssessments",
                column: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthAssessments");
        }
    }
}
