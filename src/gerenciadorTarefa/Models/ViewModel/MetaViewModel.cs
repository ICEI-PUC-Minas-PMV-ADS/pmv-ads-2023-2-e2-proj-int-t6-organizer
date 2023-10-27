using System.ComponentModel.DataAnnotations;

namespace gerenciadorTarefa.Models.ViewModel
{
    public class MetaViewModel
    {
        public int Id { get; set; }

        public Categoria Categoria { get; set; }

        public string Titulo { get; set; }

        public DateTime Prazo { get; set; }

        public int Status { get; set; }

        public DateTime DataRegistro { get; set; }

        public int UsuarioId { get; set; }

        public List<TarefaViewModel> Tarefas { get; set; }

    }

    public class TarefaViewModel

    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public bool Status { get; set; }

        public DateTime DataCriacao { get; set; }

    }
}
