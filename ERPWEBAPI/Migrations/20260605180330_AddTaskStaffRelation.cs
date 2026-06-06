using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWEBAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskStaffRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Staff_AssignedToId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedToId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "Tasks");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$wnbAMKW6CF81GjKsTCQuUeJgmQ10x2xtjFP8okuwSdIrINXHs/BDa");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToStaffId",
                table: "Tasks",
                column: "AssignedToStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Staff_AssignedToStaffId",
                table: "Tasks",
                column: "AssignedToStaffId",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Staff_AssignedToStaffId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedToStaffId",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "AssignedToId",
                table: "Tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$hiRuleSjVvshN4sGp3uIr.eAfe5fOQzSjH/2tNM7RFwDvgi/GmeUO");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToId",
                table: "Tasks",
                column: "AssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Staff_AssignedToId",
                table: "Tasks",
                column: "AssignedToId",
                principalTable: "Staff",
                principalColumn: "Id");
        }
    }
}
