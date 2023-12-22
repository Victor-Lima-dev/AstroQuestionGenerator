using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Lista : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ListaId",
                table: "Perguntas",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "Listas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Nome = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descricao = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Perguntas_ListaId",
                table: "Perguntas",
                column: "ListaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Perguntas_Listas_ListaId",
                table: "Perguntas",
                column: "ListaId",
                principalTable: "Listas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Perguntas_Listas_ListaId",
                table: "Perguntas");

            migrationBuilder.DropTable(
                name: "Listas");

            migrationBuilder.DropIndex(
                name: "IX_Perguntas_ListaId",
                table: "Perguntas");

            migrationBuilder.DropColumn(
                name: "ListaId",
                table: "Perguntas");
        }
    }
}
