using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulsePanel.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServerCheckField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CheckPort",
                table: "Servers",
                type: "integer",
                nullable: false,
                defaultValue: 80);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCheckAt",
                table: "Servers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastCheckMessage",
                table: "Servers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastResponseTimeMs",
                table: "Servers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckPort",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "LastCheckAt",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "LastCheckMessage",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "LastResponseTimeMs",
                table: "Servers");
        }
    }
}
