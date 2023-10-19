using System.ComponentModel.DataAnnotations;

namespace gerenciadorTarefa.Models.ViewModel
{
    public class MetaViewModel
    {
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "Informe o Titulo")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Informe o Prazo")]
        public DateTime Prazo { get; set; }

        [Required(ErrorMessage = "Informe o Nome")]
        [Display(Name ="Tarefa")]
        public string Nome { get; set; }

        public bool Status { get; set; }

    }
}
