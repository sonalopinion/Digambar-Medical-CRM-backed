using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWEBAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePerformanceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Performances_Staff_StaffId",
                table: "Performances");

            migrationBuilder.DropIndex(
                name: "IX_Performances_StaffId",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "ReviewDate",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Performances");

            migrationBuilder.AddColumn<string>(
                name: "Marks",
                table: "Performances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Performances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$y/KgpMC3NP947rwbV/p4XeXJkgfs4l5K8PoS2088PuHwCE3q.Zmtm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Marks",
                table: "Performances");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Performances");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Performances",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Performances",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewDate",
                table: "Performances",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Performances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "Performances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$G4wrE0se50Xz5NNrbSbILuO/PMl8VWsFcCuxjWEWvQh01MWGPH.fq");

            migrationBuilder.CreateIndex(
                name: "IX_Performances_StaffId",
                table: "Performances",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Performances_Staff_StaffId",
                table: "Performances",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
