using System.ComponentModel.DataAnnotations;

namespace AssistenciaTecnica.Models
{
    public class Empresa
    {
        public int Id { get; set; }

        [Required]
        public string NomeEmpresa { get; set; } = "";

        [Required]
        public string Usuario { get; set; } = "";

        [Required]
        public string Senha { get; set; } = "";

        public string WhatsApp { get; set; } = "";

        public bool Ativo { get; set; } = true;

        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}