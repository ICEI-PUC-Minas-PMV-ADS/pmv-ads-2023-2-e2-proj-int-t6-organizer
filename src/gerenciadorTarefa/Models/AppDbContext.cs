using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using gerenciadorTarefa.Models.ViewModel;

namespace gerenciadorTarefa.Models
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Meta> Metas { get; set; }

        public DbSet<Tarefa> Tarefas { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<gerenciadorTarefa.Models.ViewModel.MetaViewModel> MetaViewModel { get; set; }
    }
}
