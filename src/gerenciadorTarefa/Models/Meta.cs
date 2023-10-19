using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gerenciadorTarefa.Models
{
    [Table("Metas")]
    public class Meta
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a Categoria")]
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "Informe o Titulo")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Informe o Prazo")]
        public DateTime Prazo { get; set; }

        public int Status { get; set; }

        public DateTime DataRegistro { get; set; } = DateTime.Now;

        public int UsuarioId { get; set; }
     
        public Usuario Usuario { get; set; }

        public ICollection<Tarefa> Tarefas { get; set; }

    }
    public enum Categoria { Profissional, Academico, Pessoal }
}

