using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheProjector.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectArchiveDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchivedUtc",
                table: "Projects",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateArchivedUtc",
                table: "Projects");
        }
    }
}
