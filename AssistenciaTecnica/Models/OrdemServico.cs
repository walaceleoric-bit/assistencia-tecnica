using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssistenciaTecnica.Models
{
    public class OrdemServico
    {
        public int Id { get; set; }

        public int EmpresaId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        public Cliente? Cliente { get; set; }

        public int? ServicoId { get; set; }

        public Servico? Servico { get; set; }

        [Required]
        public string Aparelho { get; set; } = "";

        public string FotoUrl { get; set; } = "";

        public string MarcaModelo { get; set; } = "";

        public string DefeitoRelatado { get; set; } = "";

        public string ObservacaoTecnica { get; set; } = "";

        public string Status { get; set; } = "Aberta";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;

        public DateTime? DataFinalizacao { get; set; }
    }
}