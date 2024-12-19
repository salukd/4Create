using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinica.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationInDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ValidationSchemas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 18, 17, 12, 5, 324, DateTimeKind.Utc).AddTicks(2290),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 12, 18, 18, 46, 54, 142, DateTimeKind.Local).AddTicks(9030));

            migrationBuilder.AddColumn<int>(
                name: "DurationInDays",
                table: "Trials",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInDays",
                table: "Trials");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ValidationSchemas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 18, 18, 46, 54, 142, DateTimeKind.Local).AddTicks(9030),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 12, 18, 17, 12, 5, 324, DateTimeKind.Utc).AddTicks(2290));
        }
    }
}
