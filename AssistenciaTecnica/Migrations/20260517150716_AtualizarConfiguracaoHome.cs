using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenciaTecnica.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarConfiguracaoHome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CorTexto",
                table: "Configuracoes",
                newName: "SubtituloEmpresa");

            migrationBuilder.RenameColumn(
                name: "CorSecundaria",
                table: "Configuracoes",
                newName: "LogoUrl");

            migrationBuilder.RenameColumn(
                name: "CorPrimaria",
                table: "Configuracoes",
                newName: "Card3ImagemUrl");

            migrationBuilder.RenameColumn(
                name: "CorFundo",
                table: "Configuracoes",
                newName: "Card2ImagemUrl");

            migrationBuilder.AddColumn<string>(
                name: "Card1ImagemUrl",
                table: "Configuracoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Card1ImagemUrl",
                table: "Configuracoes");

            migrationBuilder.RenameColumn(
                name: "SubtituloEmpresa",
                table: "Configuracoes",
                newName: "CorTexto");

            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "Configuracoes",
                newName: "CorSecundaria");

            migrationBuilder.RenameColumn(
                name: "Card3ImagemUrl",
                table: "Configuracoes",
                newName: "CorPrimaria");

            migrationBuilder.RenameColumn(
                name: "Card2ImagemUrl",
                table: "Configuracoes",
                newName: "CorFundo");
        }
    }
}
