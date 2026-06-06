using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWEBAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSurpriseRewardsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$eZezXou48pj2brQWX4nShuC.B2lYeWMfHBpofXJZfuXjjOBrbUloW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$wlEQln4pqcVl2UMcUf3WxeT5T2tccU13pyztqKMfi9Xd56COyFbJO");
        }
    }
}
