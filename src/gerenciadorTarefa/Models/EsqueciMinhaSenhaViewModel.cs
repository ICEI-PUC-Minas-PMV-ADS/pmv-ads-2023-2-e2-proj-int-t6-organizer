using System.ComponentModel.DataAnnotations;

namespace gerenciadorTarefa.Models
{
    public class EsqueciMinhaSenhaViewModel
    {
        internal readonly string Senha;

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }
}
