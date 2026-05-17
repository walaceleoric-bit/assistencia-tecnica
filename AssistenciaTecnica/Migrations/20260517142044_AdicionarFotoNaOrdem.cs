using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenciaTecnica.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarFotoNaOrdem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "OrdensServico",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "OrdensServico");
        }
    }
}
