using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWEBAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateDSI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DSIRecords");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "DSIRecords");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "DSIRecords",
                newName: "Priority");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "DSIRecords",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "DSIRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSolved",
                table: "DSIRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "DSIRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Problem",
                table: "DSIRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$k04MJgrJu/MHgvYXnc5NDeg91/HtJIlKYYELUnoZG85Vi5gu4AxHi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "DSIRecords");

            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "DSIRecords");

            migrationBuilder.DropColumn(
                name: "IsSolved",
                table: "DSIRecords");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "DSIRecords");

            migrationBuilder.DropColumn(
                name: "Problem",
                table: "DSIRecords");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "DSIRecords",
                newName: "Score");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DSIRecords",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "DSIRecords",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$7X6O43Ykwxa0NFZOI1YXAeuYsGvGQUjLhgtFwImDh1CFjCV.2gOs2");
        }
    }
}
