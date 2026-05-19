using System.ComponentModel.DataAnnotations;

namespace AssistenciaTecnica.Models
{
    public class Servico
    {
        public int Id { get; set; }

        public int EmpresaId { get; set; }

        [Required]
        public string Nome { get; set; } = "";

        public string Descricao { get; set; } = "";

        public bool Ativo { get; set; } = true;
    }
}