using Microsoft.EntityFrameworkCore.Migrations;

namespace mahjop.Migrations
{
    public partial class dsawweq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "WorkingHours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WorkingHours_DoctorId",
                table: "WorkingHours",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkingHours_Doctors_DoctorId",
                table: "WorkingHours",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkingHours_Doctors_DoctorId",
                table: "WorkingHours");

            migrationBuilder.DropIndex(
                name: "IX_WorkingHours_DoctorId",
                table: "WorkingHours");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "WorkingHours");
        }
    }
}
