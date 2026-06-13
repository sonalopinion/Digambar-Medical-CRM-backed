using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPWEBAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffRewards_Rewards_RewardId",
                table: "StaffRewards");

            migrationBuilder.AlterColumn<int>(
                name: "RewardId",
                table: "StaffRewards",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "StaffRewards",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PointsValue",
                table: "StaffRewards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$iBkXJ49TSB66va7OH0/jP.n4NX/MEU3D2LoQZuNWWQ.bt4K4nkSIG");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffRewards_Rewards_RewardId",
                table: "StaffRewards",
                column: "RewardId",
                principalTable: "Rewards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffRewards_Rewards_RewardId",
                table: "StaffRewards");

            migrationBuilder.DropColumn(
                name: "PointsValue",
                table: "StaffRewards");

            migrationBuilder.AlterColumn<int>(
                name: "RewardId",
                table: "StaffRewards",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "StaffRewards",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$axiIZ3wjB360LArhDBxyoewDP49N.4O2dsUIY6iAwBBAj2Tq2iCYC");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffRewards_Rewards_RewardId",
                table: "StaffRewards",
                column: "RewardId",
                principalTable: "Rewards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
