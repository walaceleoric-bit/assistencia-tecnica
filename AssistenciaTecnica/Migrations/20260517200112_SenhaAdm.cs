using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenciaTecnica.Migrations
{
    /// <inheritdoc />
    public partial class SenhaAdm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenhaAdm",
                table: "Configuracoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenhaAdm",
                table: "Configuracoes");
        }
    }
}
