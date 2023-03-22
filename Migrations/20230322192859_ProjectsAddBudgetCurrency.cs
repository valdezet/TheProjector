using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheProjector.Migrations
{
    /// <inheritdoc />
    public partial class ProjectsAddBudgetCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BudgetCurrencyCode",
                table: "Projects",
                type: "nchar(3)",
                nullable: false,
                defaultValue: "");

            var currencyCode = System.Globalization.RegionInfo.CurrentRegion.ISOCurrencySymbol;
            migrationBuilder.Sql($"UPDATE Projects SET BudgetCurrencyCode='{currencyCode}'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BudgetCurrencyCode",
                table: "Projects");
        }
    }
}
