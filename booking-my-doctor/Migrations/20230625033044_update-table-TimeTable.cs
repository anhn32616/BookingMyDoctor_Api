using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace booking_my_doctor.Migrations
{
    public partial class updatetableTimeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_timetable_Doctors_DoctorId",
                table: "timetable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_timetable",
                table: "timetable");

            migrationBuilder.RenameTable(
                name: "timetable",
                newName: "Timetables");

            migrationBuilder.RenameIndex(
                name: "IX_timetable_DoctorId",
                table: "Timetables",
                newName: "IX_Timetables_DoctorId");

            migrationBuilder.AddColumn<double>(
                name: "Cost",
                table: "Timetables",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Timetables",
                table: "Timetables",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Timetables_Doctors_DoctorId",
                table: "Timetables",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timetables_Doctors_DoctorId",
                table: "Timetables");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Timetables",
                table: "Timetables");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Timetables");

            migrationBuilder.RenameTable(
                name: "Timetables",
                newName: "timetable");

            migrationBuilder.RenameIndex(
                name: "IX_Timetables_DoctorId",
                table: "timetable",
                newName: "IX_timetable_DoctorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_timetable",
                table: "timetable",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_timetable_Doctors_DoctorId",
                table: "timetable",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
