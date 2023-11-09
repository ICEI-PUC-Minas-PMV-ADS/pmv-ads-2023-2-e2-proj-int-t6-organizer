using System.Collections.Generic;
using gerenciadorTarefa.Models;

namespace gerenciadorTarefa.Models.ViewModel
{
    public class UsuarioMetaViewModel
    {
        public Usuario Usuario { get; set; }
        public List<Meta> Metas { get; set; }
    }
}