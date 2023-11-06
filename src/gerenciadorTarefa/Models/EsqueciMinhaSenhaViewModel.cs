using System.ComponentModel.DataAnnotations;

namespace gerenciadorTarefa.Models
{
    public class EsqueciMinhaSenhaViewModel
    {
        internal readonly string Senha;

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "O campo {0} é de preenchimento obrigatório.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
