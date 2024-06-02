using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_back_end_Aahad_A.Migrations.Portfolio
{
    /// <inheritdoc />
    public partial class AddInvestmentsToPortfolio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_Logins_LoginId",
                table: "Portfolios");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropIndex(
                name: "IX_Portfolios_LoginId",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "LoginId",
                table: "Portfolios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoginId",
                table: "Portfolios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    base64Credentials = table.Column<string>(type: "TEXT", nullable: false),
                    hash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_LoginId",
                table: "Portfolios",
                column: "LoginId");

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_Logins_LoginId",
                table: "Portfolios",
                column: "LoginId",
                principalTable: "Logins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
