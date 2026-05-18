using System.ComponentModel.DataAnnotations;

namespace AssistenciaTecnica.Models
{
    public class Configuracao
    {
        public int Id { get; set; }

        public int EmpresaId { get; set; }

        [Required]
        public string SenhaAdm { get; set; } = "123456";

        public string NomeEmpresa { get; set; } = "Milton Cardoso";
        public string SubtituloEmpresa { get; set; } = "Assistência Técnica";
        public string WhatsApp { get; set; } = "";
        public string CidadesAtendidas { get; set; } = "Serra e Vitória";
        public string LogoUrl { get; set; } = "";

        public string TituloPrincipal { get; set; } = "";
        public string TextoPrincipal { get; set; } = "";

        public string Destaque1Titulo { get; set; } = "";
        public string Destaque1Texto { get; set; } = "";
        public string Destaque1ImagemUrl { get; set; } = "";

        public string Destaque2Titulo { get; set; } = "";
        public string Destaque2Texto { get; set; } = "";
        public string Destaque2ImagemUrl { get; set; } = "";
    }
}