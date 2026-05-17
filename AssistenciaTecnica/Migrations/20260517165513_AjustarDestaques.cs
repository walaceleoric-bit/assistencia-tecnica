using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenciaTecnica.Migrations
{
    /// <inheritdoc />
    public partial class AjustarDestaques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Card1ImagemUrl",
                table: "Configuracoes");

            migrationBuilder.DropColumn(
                name: "Card1Texto",
                table: "Configuracoes");

            migrationBuilder.DropColumn(
                name: "Card1Titulo",
                table: "Configuracoes");

            migrationBuilder.DropColumn(
                name: "Card2ImagemUrl",
                table: "Configuracoes");

            migrationBuilder.RenameColumn(
                name: "ImagemPrincipalUrl",
                table: "Configuracoes",
                newName: "Destaque2Titulo");

            migrationBuilder.RenameColumn(
                name: "Card3Titulo",
                table: "Configuracoes",
                newName: "Destaque2Texto");

            migrationBuilder.RenameColumn(
                name: "Card3Texto",
                table: "Configuracoes",
                newName: "Destaque2ImagemUrl");

            migrationBuilder.RenameColumn(
                name: "Card3ImagemUrl",
                table: "Configuracoes",
                newName: "Destaque1Titulo");

            migrationBuilder.RenameColumn(
                name: "Card2Titulo",
                table: "Configuracoes",
                newName: "Destaque1Texto");

            migrationBuilder.RenameColumn(
                name: "Card2Texto",
                table: "Configuracoes",
                newName: "Destaque1ImagemUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Destaque2Titulo",
                table: "Configuracoes",
                newName: "ImagemPrincipalUrl");

            migrationBuilder.RenameColumn(
                name: "Destaque2Texto",
                table: "Configuracoes",
                newName: "Card3Titulo");

            migrationBuilder.RenameColumn(
                name: "Destaque2ImagemUrl",
                table: "Configuracoes",
                newName: "Card3Texto");

            migrationBuilder.RenameColumn(
                name: "Destaque1Titulo",
                table: "Configuracoes",
                newName: "Card3ImagemUrl");

            migrationBuilder.RenameColumn(
                name: "Destaque1Texto",
                table: "Configuracoes",
                newName: "Card2Titulo");

            migrationBuilder.RenameColumn(
                name: "Destaque1ImagemUrl",
                table: "Configuracoes",
                newName: "Card2Texto");

            migrationBuilder.AddColumn<string>(
                name: "Card1ImagemUrl",
                table: "Configuracoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Card1Texto",
                table: "Configuracoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Card1Titulo",
                table: "Configuracoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Card2ImagemUrl",
                table: "Configuracoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
