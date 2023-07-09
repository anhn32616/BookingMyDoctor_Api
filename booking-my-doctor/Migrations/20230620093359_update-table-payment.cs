using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace booking_my_doctor.Migrations
{
    public partial class updatetablepayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Payments_PaymentId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PaymentId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Appointments");

            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                table: "Appointments",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Appointments");

            migrationBuilder.AddColumn<DateTime>(
                name: "Month",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PaymentId",
                table: "Appointments",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Payments_PaymentId",
                table: "Appointments",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");
        }
    }
}
