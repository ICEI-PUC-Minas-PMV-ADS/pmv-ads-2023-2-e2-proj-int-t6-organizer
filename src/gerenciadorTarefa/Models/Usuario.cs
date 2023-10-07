﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gerenciadorTarefa.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Informe o nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage ="Informe o e-mail")]
        public string Email { get; set;}

        [Required(ErrorMessage ="Informe a senha")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }


        [Required(ErrorMessage = "Repita a senha")]
        [DataType(DataType.Password)]
        public string ConfirmarSenha { get; set; }

    }
}
