using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWEBAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$ymUv9BQZVMDdA18hfai1juxCVOdoPCq.JzNavr89sY.V4CJt8GpuK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$mIh9VrzlkDOn.pXOFi1cdeAsNYf7.uJyfy48s/ObPvwVEMSN0aHbu");
        }
    }
}
