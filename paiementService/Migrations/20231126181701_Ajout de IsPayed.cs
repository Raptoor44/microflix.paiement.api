using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace paiementService.Migrations
{
    /// <inheritdoc />
    public partial class AjoutdeIsPayed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPayed",
                table: "Paiements",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPayed",
                table: "Paiements");
        }
    }
}
