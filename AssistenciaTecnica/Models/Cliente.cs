using System.ComponentModel.DataAnnotations;

namespace AssistenciaTecnica.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        public int EmpresaId { get; set; }

        [Required(ErrorMessage = "Informe o nome.")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "Informe o CPF.")]
        public string Cpf { get; set; } = "";

        public string Telefone { get; set; } = "";

        public string Email { get; set; } = "";

        public string Endereco { get; set; } = "";

        public string Cidade { get; set; } = "";

        public string Observacao { get; set; } = "";

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}