using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWEBAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateFeedbackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Staff_StaffId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "FeedbackType",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Feedbacks");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "Feedbacks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Option1",
                table: "Feedbacks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Option2",
                table: "Feedbacks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Option3",
                table: "Feedbacks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Option4",
                table: "Feedbacks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "Feedbacks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Rg2ZA0NY8jxTijp.rFKwD.oE3akZsYPIKE7aCn8zXdG1PiaozsciS");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Staff_StaffId",
                table: "Feedbacks",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Staff_StaffId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Option1",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Option2",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Option3",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Option4",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "Feedbacks");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "Feedbacks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeedbackType",
                table: "Feedbacks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Feedbacks",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$y/KgpMC3NP947rwbV/p4XeXJkgfs4l5K8PoS2088PuHwCE3q.Zmtm");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Staff_StaffId",
                table: "Feedbacks",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
