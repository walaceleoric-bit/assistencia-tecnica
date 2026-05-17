using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AssistenciaTecnica.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome.")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "Informe o CPF.")]
        [CpfValido]
        public string Cpf { get; set; } = "";

        public string Telefone { get; set; } = "";

        public string Email { get; set; } = "";

        public string Endereco { get; set; } = "";

        public string Cidade { get; set; } = "";

        public string Observacao { get; set; } = "";

        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }

    public class CpfValidoAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            var cpf = value?.ToString();

            if (string.IsNullOrWhiteSpace(cpf))
                return new ValidationResult("CPF obrigatório.");

            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return new ValidationResult("CPF inválido.");

            if (cpf.Distinct().Count() == 1)
                return new ValidationResult("CPF inválido.");

            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf.Substring(0, 9);

            var soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            var digito = resto.ToString();
            tempCpf += digito;

            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            if (!cpf.EndsWith(digito))
                return new ValidationResult("CPF inválido.");

            return ValidationResult.Success;
        }
    }
}