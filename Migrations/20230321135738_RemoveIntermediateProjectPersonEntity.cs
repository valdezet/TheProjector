using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheProjector.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIntermediateProjectPersonEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonProject",
                columns: table => new
                {
                    AssignedPeopleId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedProjectsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonProject", x => new { x.AssignedPeopleId, x.AssignedProjectsId });
                    table.ForeignKey(
                        name: "FK_PersonProject_People_AssignedPeopleId",
                        column: x => x.AssignedPeopleId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonProject_Projects_AssignedProjectsId",
                        column: x => x.AssignedProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonProject_AssignedProjectsId",
                table: "PersonProject",
                column: "AssignedProjectsId");


            migrationBuilder.Sql("INSERT INTO PersonProject(AssignedPeopleId, AssignedProjectsId) SELECT PersonId, ProjectId FROM PersonProjectAssignments");

            migrationBuilder.DropTable(
                name: "PersonProjectAssignments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.Sql("INSERT INTO  PersonProjectAssignments (PersonId, ProjectId)  SELECT AssignedPeopleId, AssignedProjectsId FROM PersonProject");

            migrationBuilder.DropTable(
                name: "PersonProject");
        }
    }
}
