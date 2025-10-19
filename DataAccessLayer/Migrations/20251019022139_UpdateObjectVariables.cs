using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FishFarm.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateObjectVariables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Device",
                newName: "DeviceId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.RenameColumn(
                name: "DeviceId",
                table: "Device",
                newName: "Token");

        }
    }
}
