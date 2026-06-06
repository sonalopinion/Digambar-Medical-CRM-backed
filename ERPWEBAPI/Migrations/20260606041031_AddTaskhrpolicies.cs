using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWEBAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskhrpolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "HRPolicies");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "HRPolicies");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "HRPolicies");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "HRPolicies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GoogleDriveLink",
                table: "HRPolicies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PolicyName",
                table: "HRPolicies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$njy6kmYmpc9IgQtmTXg5O.VsOsQ84Vco7SXXqPdRvJ/d748IMiOMi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "HRPolicies");

            migrationBuilder.DropColumn(
                name: "GoogleDriveLink",
                table: "HRPolicies");

            migrationBuilder.DropColumn(
                name: "PolicyName",
                table: "HRPolicies");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "HRPolicies",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "HRPolicies",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "HRPolicies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$wnbAMKW6CF81GjKsTCQuUeJgmQ10x2xtjFP8okuwSdIrINXHs/BDa");
        }
    }
}
