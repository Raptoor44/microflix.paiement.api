using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace paiementService.Migrations
{
    /// <inheritdoc />
    public partial class AjoutdelacolonneMontantdanslatablePaiement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Montant",
                table: "Paiements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Montant",
                table: "Paiements");
        }
    }
}
