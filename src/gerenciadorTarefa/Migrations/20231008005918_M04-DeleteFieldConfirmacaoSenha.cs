using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gerenciadorTarefa.Migrations
{
    public partial class M04DeleteFieldConfirmacaoSenha : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmarSenha",
                table: "Usuarios");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfirmarSenha",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
