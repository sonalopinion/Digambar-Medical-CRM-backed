using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWEBAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStaffShiftTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffShifts_Shifts_ShiftId",
                table: "StaffShifts");

            migrationBuilder.DropColumn(
                name: "AssignedDate",
                table: "StaffShifts");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftId",
                table: "StaffShifts",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "StaffShifts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "StaffShifts",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShiftType",
                table: "StaffShifts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "StaffShifts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "StaffShifts",
                type: "interval",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$S5iOXhGYFbojOa./9aJyWeNdhVzU5kRKRPtQUVjUoL.oBGAZsT.Ee");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffShifts_Shifts_ShiftId",
                table: "StaffShifts",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffShifts_Shifts_ShiftId",
                table: "StaffShifts");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "StaffShifts");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "StaffShifts");

            migrationBuilder.DropColumn(
                name: "ShiftType",
                table: "StaffShifts");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "StaffShifts");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "StaffShifts");

            migrationBuilder.AlterColumn<int>(
                name: "ShiftId",
                table: "StaffShifts",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedDate",
                table: "StaffShifts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$l19c39ggqgFBgnxvxRPWhujiuWWjdOZFARNUaOLKLW64rXFUT3RRu");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffShifts_Shifts_ShiftId",
                table: "StaffShifts",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
