using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheProjector.Migrations
{
    /// <inheritdoc />
    public partial class PersonProjectAssignmentsManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonProjectAssignments",
                columns: table => new
                {
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    ProjectId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonProjectAssignments", x => new { x.PersonId, x.ProjectId });
                    table.ForeignKey(
                        name: "FK_PersonProjectAssignments_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonProjectAssignments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonProjectAssignments_ProjectId",
                table: "PersonProjectAssignments",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonProjectAssignments");
        }
    }
}
