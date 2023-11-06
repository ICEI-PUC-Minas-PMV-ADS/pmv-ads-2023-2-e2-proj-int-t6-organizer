using System.ComponentModel.DataAnnotations;

namespace gerenciadorTarefa.Models
{
    public class RedefinirSenhaViewModel
    {
        [Required]
        public string Token { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage ="O campo {0} é de preenchimento obrigatório.")]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; }

        [Display(Name = "Nova Senha")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        public string NovaSenha { get; set; }

        [Display(Name = "Confirmação da Nova Senha")]
        [DataType(DataType.Password)]
        [Compare(nameof(NovaSenha), ErrorMessage = "As senhas não coincidem.")]
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]

        public string ConfNovaSenha { get; set; }

    }
}
