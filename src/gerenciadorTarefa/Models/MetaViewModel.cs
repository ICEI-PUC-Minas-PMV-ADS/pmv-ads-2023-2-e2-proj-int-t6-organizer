using System.ComponentModel.DataAnnotations;

namespace gerenciadorTarefa.Models.ViewModel
{
    public class MetaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a Categoria")]
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "Informe o Titulo")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Informe o Prazo")]
        public DateTime Prazo { get; set; } = DateTime.Now.Date;

        public int Status { get; set; }

        public DateTime DataRegistro { get; set; } = DateTime.Now;

        public string UsuarioId { get; set; }

        public List<TarefaViewModel> Tarefas { get; set; }
    }

    public class TarefaViewModel

    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a Tarefa")]
        public string Nome { get; set; }

        public bool Status { get; set; }

        public DateTime DataCriacao { get; set; }

    }
}