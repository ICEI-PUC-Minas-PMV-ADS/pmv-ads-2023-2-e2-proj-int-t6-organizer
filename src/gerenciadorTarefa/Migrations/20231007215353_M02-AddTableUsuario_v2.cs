using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gerenciadorTarefa.Migrations
{
    public partial class M02AddTableUsuario_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Usuarios",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Usuarios",
                newName: "Nome");
        }
    }
}
