using System.ComponentModel.DataAnnotations;

namespace gerenciadorTarefa.Models
{
    public class Usuario
    {
        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage ="Informe seu nome")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Informe seu e-mail")]
        [EmailAddress(ErrorMessage = "Informe um email válido.")]
        public string Email { get; set;}

        [Required(ErrorMessage ="Informe a senha")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Repita a senha")]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "A senha e a confirmação de senha não coincidem.")]
        public string ConfirmarSenha { get; set; }

    }
}
