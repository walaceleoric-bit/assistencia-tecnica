using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenciaTecnica.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuracoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeEmpresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhatsApp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CidadesAtendidas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TituloPrincipal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextoPrincipal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagemPrincipalUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorPrimaria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorSecundaria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorFundo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorTexto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Card1Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Card1Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Card2Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Card2Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Card3Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Card3Texto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuracoes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuracoes");
        }
    }
}
