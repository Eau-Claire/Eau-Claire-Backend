using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FishFarm.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Device",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Device_UserId1",
                table: "Device",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Users_UserId1",
                table: "Device",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_Users_UserId1",
                table: "Device");

            migrationBuilder.DropIndex(
                name: "IX_Device_UserId1",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Device");
        }
    }
}
